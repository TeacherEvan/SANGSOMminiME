# Sangsom Mini-Me Verification System

## Quick Start

```bash
# Install dependencies
npm install

# Run full verification
npm run verify
```

## What Gets Verified

### ✅ Critical Checks (Must Pass)
- **Project Structure**: Required directories exist
- **Unity Files**: Core C# scripts are present
- **Python Syntax**: All Blender scripts compile without errors

### ⚠️ Non-Critical Checks (Logged to JSON)
- **Markdown Formatting**: Style issues (code blocks, headings, lists)
- **Python Warnings**: Non-blocking issues

## Output

### Terminal
Shows real-time verification status with color-coded results:
- 🟢 Green: Passed
- 🔴 Red: Critical error
- 🟡 Yellow: Non-critical warning

### non-critical-errors.json
Stores all non-critical issues for tracking:
- Markdown linting warnings
- Python style warnings
- Summary statistics
- Last verification timestamp

## Available Scripts

```bash
# Full verification suite
npm run verify

# Individual checks
npm run lint:markdown    # Check markdown files
npm run lint:python      # Check Python syntax
npm run check:all        # Run all linters
```

## Markdown Linting Configuration

The `.markdownlint.json` file disables non-critical formatting rules:
- MD040: Fenced code language specification
- MD041: First line heading requirement
- MD024: Duplicate heading detection
- MD025: Multiple H1 headings
- MD022/MD032: Blank lines around elements
- MD029: Ordered list prefixes
- MD034: Bare URL usage
- MD030: List marker spacing

These are style preferences, not errors affecting documentation quality.

## Exit Codes

- `0`: All critical checks passed
- `1`: One or more critical errors found

Non-critical issues don't affect the exit code but are tracked in `non-critical-errors.json`.

## Integration

### CI/CD

```yaml
# Example GitHub Actions
- name: Verify Project
  run: npm run verify
```

### Pre-commit Hook

```bash
#!/bin/sh
npm run verify
```

## Troubleshooting

**Q: Verification fails with "python not found"**
A: Ensure Python 3.11+ is installed and in PATH

**Q: Markdown linting shows too many errors**
A: Non-critical errors are now in `non-critical-errors.json`. Only blocking issues fail verification.

**Q: How do I fix non-critical errors?**
A: They're tracked for reference but don't block development. Address them gradually or update `.markdownlint.json` to adjust rules.
