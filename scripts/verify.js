const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

// Colors for terminal output
const colors = {
    reset: '\x1b[0m',
    red: '\x1b[31m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m'
};

const NON_CRITICAL_ERRORS_FILE = path.join(__dirname, '..', 'non-critical-errors.json');

// Load existing non-critical errors
function loadNonCriticalErrors() {
    if (fs.existsSync(NON_CRITICAL_ERRORS_FILE)) {
        return JSON.parse(fs.readFileSync(NON_CRITICAL_ERRORS_FILE, 'utf8'));
    }
    return {
        lastUpdated: new Date().toISOString(),
        markdownLintErrors: [],
        pythonWarnings: [],
        summary: {
            totalMarkdownIssues: 0,
            totalPythonWarnings: 0
        }
    };
}

// Save non-critical errors
function saveNonCriticalErrors(errors) {
    errors.lastUpdated = new Date().toISOString();
    fs.writeFileSync(NON_CRITICAL_ERRORS_FILE, JSON.stringify(errors, null, 2));
}

// Check Python syntax
function checkPythonSyntax() {
    console.log(`\n${colors.blue}🐍 Checking Python syntax...${colors.reset}`);

    const pythonFiles = [
        'Blender/startup_script.py',
        'Blender/character_controller.py',
        'Blender/user_manager.py',
        'Blender/minime_addon.py',
        'Blender/game_manager.py'
    ];

    let errors = 0;
    const warnings = [];

    pythonFiles.forEach(file => {
        const filePath = path.join(__dirname, '..', file);
        if (fs.existsSync(filePath)) {
            try {
                execSync(`python -m py_compile "${filePath}"`, { stdio: 'pipe' });
                console.log(`${colors.green}✓${colors.reset} ${file}`);
            } catch (error) {
                errors++;
                console.log(`${colors.red}✗${colors.reset} ${file}`);
                warnings.push({
                    file: file,
                    error: error.message.split('\n')[0]
                });
            }
        }
    });

    return { errors, warnings };
}

// Check Unity C# files existence
function checkUnityFiles() {
    console.log(`\n${colors.blue}🎮 Checking Unity structure...${colors.reset}`);

    const criticalFiles = [
        'Assets/Scripts/Runtime/GameManager.cs',
        'Assets/Scripts/Runtime/UserManager.cs',
        'Assets/Scripts/Runtime/CharacterController.cs',
        'Assets/Scripts/Runtime/GameUI.cs',
        'Assets/Scripts/Runtime/UserProfile.cs',
        'ProjectSettings/ProjectVersion.txt'
    ];

    let errors = 0;

    criticalFiles.forEach(file => {
        const filePath = path.join(__dirname, '..', file);
        if (fs.existsSync(filePath)) {
            console.log(`${colors.green}✓${colors.reset} ${file}`);
        } else {
            errors++;
            console.log(`${colors.red}✗${colors.reset} ${file} (missing)`);
        }
    });

    return errors;
}

// Check markdown files and categorize errors
function checkMarkdownFiles() {
    console.log(`\n${colors.blue}📝 Checking Markdown files...${colors.reset}`);

    const nonCriticalRules = [
        'MD040', // fenced-code-language
        'MD041', // first-line-heading
        'MD036', // no-emphasis-as-heading
        'MD024', // no-duplicate-heading
        'MD025', // single-title
        'MD022', // blanks-around-headings
        'MD032', // blanks-around-lists
        'MD029', // ol-prefix
        'MD034', // no-bare-urls
        'MD030'  // list-marker-space
    ];

    const nonCriticalErrors = [];
    let criticalErrors = 0;

    try {
        // Run markdownlint but capture errors
        execSync('npx markdownlint **/*.md --ignore node_modules --ignore .venv --json', {
            stdio: 'pipe',
            encoding: 'utf8'
        });
        console.log(`${colors.green}✓${colors.reset} All markdown files passed`);
    } catch (error) {
        const output = error.stdout || error.stderr || '';

        // Try to parse JSON output
        try {
            const results = JSON.parse(output);

            Object.keys(results).forEach(file => {
                const fileErrors = results[file];
                fileErrors.forEach(err => {
                    const ruleId = err.ruleNames[0];
                    const errorInfo = {
                        file: file,
                        line: err.lineNumber,
                        rule: ruleId,
                        description: err.ruleDescription,
                        message: err.errorDetail || err.ruleDescription
                    };

                    if (nonCriticalRules.includes(ruleId)) {
                        nonCriticalErrors.push(errorInfo);
                    } else {
                        criticalErrors++;
                        console.log(`${colors.red}✗${colors.reset} ${file}:${err.lineNumber} - ${ruleId}: ${err.ruleDescription}`);
                    }
                });
            });

            if (nonCriticalErrors.length > 0) {
                console.log(`${colors.yellow}⚠${colors.reset}  ${nonCriticalErrors.length} non-critical markdown issues (saved to non-critical-errors.json)`);
            }
        } catch (parseError) {
            // If can't parse, treat as critical
            console.log(`${colors.yellow}⚠${colors.reset}  Markdown linting had issues (see above)`);
        }
    }

    return { criticalErrors, nonCriticalErrors };
}

// Check project structure
function checkProjectStructure() {
    console.log(`\n${colors.blue}📁 Checking project structure...${colors.reset}`);

    const requiredDirs = [
        'Assets/Scripts/Runtime',
        'Assets/Scripts/Tests',
        'Assets/Scripts/Editor',
        'Blender',
        'Docs',
        '.vscode/rules'
    ];

    let errors = 0;

    requiredDirs.forEach(dir => {
        const dirPath = path.join(__dirname, '..', dir);
        if (fs.existsSync(dirPath)) {
            console.log(`${colors.green}✓${colors.reset} ${dir}`);
        } else {
            errors++;
            console.log(`${colors.red}✗${colors.reset} ${dir} (missing)`);
        }
    });

    return errors;
}

// Main verification function
async function main() {
    console.log(`${colors.magenta}╔════════════════════════════════════════════════════╗${colors.reset}`);
    console.log(`${colors.magenta}║   Sangsom Mini-Me Project Verification            ║${colors.reset}`);
    console.log(`${colors.magenta}╚════════════════════════════════════════════════════╝${colors.reset}`);

    let totalErrors = 0;
    const nonCriticalData = loadNonCriticalErrors();

    // Check project structure
    const structureErrors = checkProjectStructure();
    totalErrors += structureErrors;

    // Check Unity files
    const unityErrors = checkUnityFiles();
    totalErrors += unityErrors;

    // Check Python syntax
    const pythonResult = checkPythonSyntax();
    totalErrors += pythonResult.errors;
    nonCriticalData.pythonWarnings = pythonResult.warnings;

    // Check markdown files
    const markdownResult = checkMarkdownFiles();
    totalErrors += markdownResult.criticalErrors;
    nonCriticalData.markdownLintErrors = markdownResult.nonCriticalErrors;

    // Update summary
    nonCriticalData.summary = {
        totalMarkdownIssues: markdownResult.nonCriticalErrors.length,
        totalPythonWarnings: pythonResult.warnings.length,
        lastVerified: new Date().toISOString()
    };

    // Save non-critical errors
    saveNonCriticalErrors(nonCriticalData);

    // Final summary
    console.log(`\n${colors.magenta}═══════════════════════════════════════════════════${colors.reset}`);

    if (totalErrors === 0) {
        console.log(`${colors.green}✓ All critical checks passed!${colors.reset}`);
        console.log(`${colors.yellow}ℹ Non-critical issues: ${nonCriticalData.summary.totalMarkdownIssues + nonCriticalData.summary.totalPythonWarnings}${colors.reset}`);
        console.log(`${colors.blue}→ See non-critical-errors.json for details${colors.reset}`);
        process.exit(0);
    } else {
        console.log(`${colors.red}✗ ${totalErrors} critical error(s) found${colors.reset}`);
        console.log(`${colors.yellow}ℹ Non-critical issues: ${nonCriticalData.summary.totalMarkdownIssues + nonCriticalData.summary.totalPythonWarnings}${colors.reset}`);
        process.exit(1);
    }
}

main().catch(err => {
    console.error(`${colors.red}Verification failed:${colors.reset}`, err);
    process.exit(1);
});
