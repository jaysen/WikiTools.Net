#!/bin/bash
# Create WikiTools.Net distrobox container
# Compatible with distrobox 1.8.2.1+

set -e

echo "🚀 Creating WikiTools.Net development container..."
echo ""

# Create the container with all options inline
distrobox create \
  --name wikitools-dev \
  --image registry.fedoraproject.org/fedora-toolbox:40 \
  --volume "$HOME/wikis:$HOME/wikis:rw" \
  --yes

echo ""
echo "✅ Container 'wikitools-dev' created successfully!"
echo ""
echo "Next steps:"
echo "  1. Enter the container: distrobox enter wikitools-dev"
echo "  2. Navigate to project: cd ~/path/to/WikiTools.Net"
echo "  3. Run setup script: ./.distrobox/setup.sh"
echo ""
