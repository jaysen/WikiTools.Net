using System;
using System.IO;
using WikiTools;
using Xunit;

namespace WikiTools.Tests;

public class ObsidianPageTests
{
    private readonly string _testFolder;

    public ObsidianPageTests()
    {
        _testFolder = TestUtilities.GetTestFolder("obsidian_page_tests");
    }

    [Fact]
    public void Constructor_ThrowsIfFileNotExists()
    {
        // Arrange
        var path = "NoSuchFile.md";

        // Assert
        Assert.Throws<FileNotFoundException>(() => new ObsidianPage(path));
    }

    [Fact]
    public void Constructor_ThrowsIfWrongExtension()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "wrong_extension_test.wiki");
        File.WriteAllText(path, "test");

        // Assert
        Assert.Throws<FormatException>(() => new ObsidianPage(path));
    }

    [Fact]
    public void GetHeaders_ParsesMarkdownHeaders()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "header_test.md");
        var content = @"# Main Header
Some content
## Subheader
### Third Level";
        File.WriteAllText(path, content);

        // Act
        var page = new ObsidianPage(path);
        var headers = page.GetHeaders();

        // Assert
        Assert.Equal(3, headers.Count);
        Assert.Contains("Main Header", headers);
        Assert.Contains("Subheader", headers);
        Assert.Contains("Third Level", headers);
    }

    [Fact]
    public void GetLinks_ParsesWikiLinks()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "links_test.md");
        var content = "See [[Page One]] and [[Page Two|Display Text]] for more.";
        File.WriteAllText(path, content);

        // Act
        var page = new ObsidianPage(path);
        var links = page.GetLinks();

        // Assert
        Assert.Equal(2, links.Count);
        Assert.Contains("Page One", links);
        Assert.Contains("Page Two", links);
    }

    [Fact]
    public void GetTags_ParsesInlineTags()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "inline_tags_test.md");
        var content = "This page has #tag1 and #tag2 tags.";
        File.WriteAllText(path, content);

        // Act
        var page = new ObsidianPage(path);
        var tags = page.GetTags();

        // Assert
        Assert.Equal(2, tags.Count);
        Assert.Contains("tag1", tags);
        Assert.Contains("tag2", tags);
    }

    [Fact]
    public void GetTags_ParsesFrontmatterTags()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "frontmatter_tags_test.md");
        var content = @"---
tags: [example, test]
---

# Content";
        File.WriteAllText(path, content);

        // Act
        var page = new ObsidianPage(path);
        var tags = page.GetTags();

        // Assert
        Assert.Contains("example", tags);
        Assert.Contains("test", tags);
    }

    [Fact]
    public void GetTags_CombinesFrontmatterAndInlineTags()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "combined_tags_test.md");
        var content = @"---
tags: [frontmatter]
---

Content with #inline tag.";
        File.WriteAllText(path, content);

        // Act
        var page = new ObsidianPage(path);
        var tags = page.GetTags();

        // Assert
        Assert.Equal(2, tags.Count);
        Assert.Contains("frontmatter", tags);
        Assert.Contains("inline", tags);
    }

    [Fact]
    public void GetAliases_ParsesFrontmatterAliases()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "aliases_test.md");
        var content = @"---
aliases: [""Alias One"", ""Alias Two""]
---

# Content";
        File.WriteAllText(path, content);

        // Act
        var page = new ObsidianPage(path);
        var aliases = page.GetAliases();

        // Assert
        Assert.Equal(2, aliases.Count);
        Assert.Contains("Alias One", aliases);
        Assert.Contains("Alias Two", aliases);
    }

    [Fact]
    public void GetContent_ReadsFileContent()
    {
        // Arrange
        var path = Path.Combine(_testFolder, "content_test.md");
        var expected = "# Test\n\nSome content here.";
        File.WriteAllText(path, expected);

        // Act
        var page = new ObsidianPage(path);
        var actual = page.GetContent();

        // Assert
        Assert.Equal(expected, actual);
    }
}
