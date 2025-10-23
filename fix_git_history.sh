#!/bin/bash

# Script to remove sensitive files from git history
# WARNING: This rewrites git history. Make sure you have a backup!

echo "This script will remove appsettings.json files from git history"
echo "WARNING: This rewrites history and requires force push!"
echo ""
read -p "Do you want to continue? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Aborted."
    exit 1
fi

echo "Removing appsettings.json files from git history..."

# Use git filter-branch to remove the files
git filter-branch --force --index-filter \
  'git rm --cached --ignore-unmatch WebApi/appsettings.json WebApi/appsettings.Development.json WebApi/bin/Debug/net7.0/appsettings.json' \
  --prune-empty --tag-name-filter cat -- --all

echo ""
echo "Cleaning up..."
git for-each-ref --format='delete %(refname)' refs/original | git update-ref --stdin
git reflog expire --expire=now --all
git gc --prune=now --aggressive

echo ""
echo "IMPORTANT NEXT STEPS:"
echo "1. IMMEDIATELY rotate all exposed credentials:"
echo "   - AWS Access Keys (in AWS Console)"
echo "   - Database passwords"
echo "   - JWT secret keys"
echo ""
echo "2. Add .gitignore to git:"
echo "   git add .gitignore"
echo "   git add WebApi/appsettings.template.json"
echo "   git add WebApi/README.md"
echo "   git commit -m 'Add .gitignore and remove sensitive config files'"
echo ""
echo "3. Force push (WARNING: This affects all collaborators!):"
echo "   git push origin --force --all"
echo "   git push origin --force --tags"
echo ""
echo "4. All team members must re-clone the repository"