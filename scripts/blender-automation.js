/**
 * Blender File Watcher and Auto-Export Script
 * 
 * Watches Blender .blend files for changes and automatically triggers
 * character exports to Unity Assets folder.
 * 
 * Usage:
 *   node scripts/blender-automation.js
 *   OR
 *   npm run blender:watch
 * 
 * Features:
 *   - Watches Blender/characters/ directory recursively
 *   - Auto-exports .blend files on save
 *   - Debounces rapid saves (3 second cooldown)
 *   - Colored console output for status
 * 
 * Requirements:
 *   - Node.js 14+
 *   - Blender in system PATH (or update BLENDER_EXE constant)
 */

const fs = require('fs');
const path = require('path');
const { exec } = require('child_process');

// ============================================================================
// Configuration
// ============================================================================

const WATCH_DIR = path.join(__dirname, '..', 'Blender', 'characters');
const BLENDER_EXE = 'blender'; // Assumes 'blender' is in PATH
const EXPORT_SCRIPT = path.join(__dirname, '..', 'Blender', 'export_character.py');
const DEBOUNCE_MS = 3000; // Wait 3 seconds after last change before exporting

// ============================================================================
// Console Colors (PowerShell compatible)
// ============================================================================

const colors = {
    reset: '\x1b[0m',
    bright: '\x1b[1m',
    dim: '\x1b[2m',

    red: '\x1b[31m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m',
    cyan: '\x1b[36m',
    white: '\x1b[37m',
};

function log(message, color = 'white') {
    const timestamp = new Date().toLocaleTimeString();
    console.log(`${colors.dim}[${timestamp}]${colors.reset} ${colors[color]}${message}${colors.reset}`);
}

// ============================================================================
// File Watcher State
// ============================================================================

const pendingExports = new Map(); // filename -> timeout ID

function scheduleExport(filename) {
    const blendFile = path.join(WATCH_DIR, filename);

    // Cancel any pending export for this file
    if (pendingExports.has(filename)) {
        clearTimeout(pendingExports.get(filename));
        log(`⏱️  Debouncing export for: ${filename}`, 'yellow');
    }

    // Schedule new export after debounce period
    const timeoutId = setTimeout(() => {
        exportBlenderFile(blendFile);
        pendingExports.delete(filename);
    }, DEBOUNCE_MS);

    pendingExports.set(filename, timeoutId);
    log(`📦 Detected change: ${filename}`, 'cyan');
    log(`   Export scheduled in ${DEBOUNCE_MS / 1000}s...`, 'dim');
}

function exportBlenderFile(blendFilePath) {
    const filename = path.basename(blendFilePath);
    log(`\n${'='.repeat(60)}`, 'blue');
    log(`⚙️  Exporting: ${filename}`, 'blue');
    log(`${'='.repeat(60)}`, 'blue');

    const cmd = `"${BLENDER_EXE}" --background "${blendFilePath}" --python "${EXPORT_SCRIPT}"`;

    const startTime = Date.now();
    exec(cmd, (error, stdout, stderr) => {
        const duration = ((Date.now() - startTime) / 1000).toFixed(1);

        if (error) {
            log(`\n❌ Export failed after ${duration}s`, 'red');
            log(`   Error: ${error.message}`, 'red');
            return;
        }

        // Filter out Blender's info messages
        const filteredStderr = stderr
            .split('\n')
            .filter(line => !line.includes('Info:') && line.trim())
            .join('\n');

        if (filteredStderr) {
            log(`\n⚠️  Warnings:`, 'yellow');
            console.log(filteredStderr);
        }

        // Print Blender output
        console.log(stdout);

        log(`✅ Export complete in ${duration}s!`, 'green');
        log(`${'='.repeat(60)}\n`, 'blue');
    });
}

// ============================================================================
// File System Watcher
// ============================================================================

function startWatching() {
    // Ensure watch directory exists
    if (!fs.existsSync(WATCH_DIR)) {
        log(`❌ Watch directory does not exist: ${WATCH_DIR}`, 'red');
        log(`   Creating directory...`, 'yellow');
        fs.mkdirSync(WATCH_DIR, { recursive: true });
        log(`✅ Directory created!`, 'green');
    }

    log(`\n${'='.repeat(60)}`, 'cyan');
    log(`🔍 Blender File Watcher Started`, 'cyan');
    log(`${'='.repeat(60)}`, 'cyan');
    log(`   Directory: ${WATCH_DIR}`, 'dim');
    log(`   Export Script: ${EXPORT_SCRIPT}`, 'dim');
    log(`   Debounce: ${DEBOUNCE_MS}ms`, 'dim');
    log(`${'='.repeat(60)}\n`, 'cyan');
    log(`💡 Tip: Save changes in Blender to trigger auto-export`, 'magenta');
    log(`   Press Ctrl+C to stop watching\n`, 'dim');

    // Watch for changes
    const watcher = fs.watch(WATCH_DIR, { recursive: true }, (eventType, filename) => {
        if (!filename) return;

        // Only process .blend files (not .blend1 backups)
        if (filename.endsWith('.blend') && !filename.endsWith('.blend1')) {
            scheduleExport(filename);
        }
    });

    // Handle process termination
    process.on('SIGINT', () => {
        log(`\n⏹️  Stopping watcher...`, 'yellow');

        // Cancel pending exports
        pendingExports.forEach((timeoutId, filename) => {
            clearTimeout(timeoutId);
            log(`   Cancelled export for: ${filename}`, 'dim');
        });

        watcher.close();
        log(`✅ Watcher stopped. Goodbye!`, 'green');
        process.exit(0);
    });
}

// ============================================================================
// Validation
// ============================================================================

function validateSetup() {
    const errors = [];

    // Check if Blender is in PATH
    exec(`"${BLENDER_EXE}" --version`, (error) => {
        if (error) {
            errors.push(`Blender not found in PATH. Expected: ${BLENDER_EXE}`);
            errors.push(`Install Blender 5.0.0 or update BLENDER_EXE constant in this script.`);
        }
    });

    // Check if export script exists
    if (!fs.existsSync(EXPORT_SCRIPT)) {
        errors.push(`Export script not found: ${EXPORT_SCRIPT}`);
    }

    // Wait a moment for async exec to complete
    setTimeout(() => {
        if (errors.length > 0) {
            log(`\n❌ Setup validation failed:`, 'red');
            errors.forEach(err => log(`   - ${err}`, 'red'));
            process.exit(1);
        } else {
            startWatching();
        }
    }, 500);
}

// ============================================================================
// Entry Point
// ============================================================================

if (require.main === module) {
    validateSetup();
}

module.exports = { scheduleExport, exportBlenderFile };
