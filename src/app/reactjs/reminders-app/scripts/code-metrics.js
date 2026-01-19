#!/usr/bin/env node

const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

// Colors for output
const colors = {
    red: '\x1b[31m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m',
    cyan: '\x1b[36m',
    white: '\x1b[37m',
    reset: '\x1b[0m',
    bold: '\x1b[1m'
};

function colorize(color, text) {
    return `${colors[color]}${text}${colors.reset}`;
}

function header(text) {
    console.log('\n' + colorize('bold', '='.repeat(60)));
    console.log(colorize('bold', text.toUpperCase()));
    console.log(colorize('bold', '='.repeat(60)));
}

function subheader(text) {
    console.log('\n' + colorize('cyan', '--- ' + text + ' ---'));
}

function analyzeProjectStructure() {
    const srcPath = path.join(__dirname, '..', 'src');
    
    function countFiles(dir, extensions = ['.ts', '.tsx', '.js', '.jsx']) {
        let count = 0;
        let totalLines = 0;
        const files = [];
        
        function traverse(currentPath) {
            const items = fs.readdirSync(currentPath);
            
            for (const item of items) {
                const fullPath = path.join(currentPath, item);
                const stat = fs.statSync(fullPath);
                
                if (stat.isDirectory() && !item.startsWith('.') && item !== 'node_modules') {
                    traverse(fullPath);
                } else if (stat.isFile()) {
                    const ext = path.extname(item);
                    if (extensions.includes(ext)) {
                        count++;
                        const content = fs.readFileSync(fullPath, 'utf8');
                        const lines = content.split('\n').length;
                        totalLines += lines;
                        files.push({
                            path: path.relative(srcPath, fullPath),
                            lines: lines,
                            size: stat.size
                        });
                    }
                }
            }
        }
        
        traverse(dir);
        return { count, totalLines, files };
    }
    
    const sourceAnalysis = countFiles(srcPath);
    const testFiles = sourceAnalysis.files.filter(f => f.path.includes('.test.') || f.path.includes('.spec.'));
    const prodFiles = sourceAnalysis.files.filter(f => !f.path.includes('.test.') && !f.path.includes('.spec.'));
    
    return {
        total: sourceAnalysis,
        production: { 
            count: prodFiles.length, 
            totalLines: prodFiles.reduce((sum, f) => sum + f.lines, 0),
            files: prodFiles
        },
        test: { 
            count: testFiles.length, 
            totalLines: testFiles.reduce((sum, f) => sum + f.lines, 0),
            files: testFiles
        }
    };
}

function calculateCyclomaticComplexity() {
    const srcPath = path.join(__dirname, '..', 'src');
    const complexityData = [];
    
    function analyzeFile(filePath) {
        try {
            const content = fs.readFileSync(filePath, 'utf8');
            
            // Simple cyclomatic complexity calculation
            // Count decision points: if, else, while, for, case, catch, &&, ||, ?
            const decisionPoints = [
                /\bif\s*\(/g,
                /\belse\b/g,
                /\bwhile\s*\(/g,
                /\bfor\s*\(/g,
                /\bcase\b/g,
                /\bcatch\s*\(/g,
                /&&/g,
                /\|\|/g,
                /\?/g,
                /\bswitch\s*\(/g
            ];
            
            let complexity = 1; // Base complexity
            decisionPoints.forEach(pattern => {
                const matches = content.match(pattern);
                if (matches) {
                    complexity += matches.length;
                }
            });
            
            // Count functions/methods
            const functionMatches = content.match(/function\s+\w+|=>\s*{|:\s*\([^)]*\)\s*=>/g) || [];
            const functionCount = functionMatches.length || 1;
            
            return {
                file: path.relative(srcPath, filePath),
                complexity,
                functions: functionCount,
                avgComplexity: complexity / functionCount,
                lines: content.split('\n').length
            };
        } catch (error) {
            return null;
        }
    }
    
    function traverseDirectory(dir) {
        const items = fs.readdirSync(dir);
        
        for (const item of items) {
            const fullPath = path.join(dir, item);
            const stat = fs.statSync(fullPath);
            
            if (stat.isDirectory() && !item.startsWith('.') && item !== 'node_modules') {
                traverseDirectory(fullPath);
            } else if (stat.isFile() && /\.(ts|tsx|js|jsx)$/.test(item) && !item.includes('.test.') && !item.includes('.spec.')) {
                const analysis = analyzeFile(fullPath);
                if (analysis) {
                    complexityData.push(analysis);
                }
            }
        }
    }
    
    traverseDirectory(srcPath);
    return complexityData;
}

function calculateMaintainabilityIndex(file) {
    try {
        const content = fs.readFileSync(path.join(__dirname, '..', 'src', file.file), 'utf8');
        
        // Simplified maintainability index calculation
        const linesOfCode = content.split('\n').filter(line => line.trim() && !line.trim().startsWith('//')).length;
        const cyclomaticComplexity = file.complexity;
        
        // Halstead metrics (simplified)
        const operators = content.match(/[+\-*/=<>!&|%^~?:]/g) || [];
        const halsteadLength = operators.length;
        
        // Maintainability Index = 171 - 5.2 * ln(HalsteadVolume) - 0.23 * CyclomaticComplexity - 16.2 * ln(LinesOfCode)
        const halsteadVolume = halsteadLength * Math.log2(halsteadLength + 1);
        const maintainabilityIndex = Math.max(0, 
            171 - 5.2 * Math.log(halsteadVolume || 1) - 0.23 * cyclomaticComplexity - 16.2 * Math.log(linesOfCode || 1)
        );
        
        return Math.round(maintainabilityIndex);
    } catch (error) {
        return 0;
    }
}

function generateReport() {
    header('ReactJS Code Metrics Report');
    
    console.log(colorize('blue', `Generated on: ${new Date().toLocaleString()}`));
    console.log(colorize('blue', `Project: Reminders ReactJS Application`));
    
    subheader('Project Overview');
    const structure = analyzeProjectStructure();
    
    console.log(`${colorize('green', '📁 Total Files:')} ${structure.total.count}`);
    console.log(`${colorize('green', '📄 Production Files:')} ${structure.production.count}`);
    console.log(`${colorize('green', '🧪 Test Files:')} ${structure.test.count}`);
    console.log(`${colorize('green', '📏 Total Lines:')} ${structure.total.totalLines.toLocaleString()}`);
    console.log(`${colorize('green', '📏 Production Lines:')} ${structure.production.totalLines.toLocaleString()}`);
    console.log(`${colorize('green', '📏 Test Lines:')} ${structure.test.totalLines.toLocaleString()}`);
    console.log(`${colorize('green', '📊 Test Coverage Ratio:')} ${((structure.test.totalLines / structure.production.totalLines) * 100).toFixed(1)}%`);
    
    subheader('Cyclomatic Complexity Analysis');
    const complexityData = calculateCyclomaticComplexity();
    
    if (complexityData.length > 0) {
        const totalComplexity = complexityData.reduce((sum, f) => sum + f.complexity, 0);
        const avgComplexity = totalComplexity / complexityData.length;
        const maxComplexity = Math.max(...complexityData.map(f => f.complexity));
        const minComplexity = Math.min(...complexityData.map(f => f.complexity));
        
        console.log(`${colorize('yellow', '🔢 Average Complexity:')} ${avgComplexity.toFixed(2)}`);
        console.log(`${colorize('yellow', '📈 Maximum Complexity:')} ${maxComplexity}`);
        console.log(`${colorize('yellow', '📉 Minimum Complexity:')} ${minComplexity}`);
        
        // Show most complex files
        const mostComplex = complexityData
            .sort((a, b) => b.complexity - a.complexity)
            .slice(0, 5);
            
        console.log(colorize('red', '\n🚨 Most Complex Files:'));
        mostComplex.forEach((file, index) => {
            console.log(`  ${index + 1}. ${file.file} (Complexity: ${file.complexity})`);
        });
    }
    
    subheader('Maintainability Index');
    const maintainabilityData = complexityData.map(file => ({
        ...file,
        maintainability: calculateMaintainabilityIndex(file)
    }));
    
    if (maintainabilityData.length > 0) {
        const avgMaintainability = maintainabilityData.reduce((sum, f) => sum + f.maintainability, 0) / maintainabilityData.length;
        
        console.log(`${colorize('magenta', '🔧 Average Maintainability Index:')} ${avgMaintainability.toFixed(1)}/100`);
        
        const maintainabilityRating = avgMaintainability > 85 ? 'Excellent' : 
                                    avgMaintainability > 70 ? 'Good' : 
                                    avgMaintainability > 50 ? 'Moderate' : 'Poor';
        
        console.log(`${colorize('magenta', '📊 Maintainability Rating:')} ${maintainabilityRating}`);
        
        // Show least maintainable files
        const leastMaintainable = maintainabilityData
            .sort((a, b) => a.maintainability - b.maintainability)
            .slice(0, 5);
            
        console.log(colorize('red', '\n⚠️  Least Maintainable Files:'));
        leastMaintainable.forEach((file, index) => {
            console.log(`  ${index + 1}. ${file.file} (MI: ${file.maintainability})`);
        });
    }
    
    subheader('File Size Analysis');
    const largeFiles = structure.production.files
        .sort((a, b) => b.lines - a.lines)
        .slice(0, 5);
        
    console.log(colorize('cyan', '📏 Largest Files by Lines:'));
    largeFiles.forEach((file, index) => {
        console.log(`  ${index + 1}. ${file.path} (${file.lines} lines)`);
    });
    
    subheader('Code Quality Recommendations');
    
    const highComplexityFiles = complexityData.filter(f => f.complexity > 10);
    const lowMaintainabilityFiles = maintainabilityData.filter(f => f.maintainability < 50);
    const largeFilesCount = structure.production.files.filter(f => f.lines > 200).length;
    
    if (highComplexityFiles.length > 0) {
        console.log(colorize('red', `⚠️  ${highComplexityFiles.length} files have high complexity (>10)`));
    }
    
    if (lowMaintainabilityFiles.length > 0) {
        console.log(colorize('red', `⚠️  ${lowMaintainabilityFiles.length} files have low maintainability (<50)`));
    }
    
    if (largeFilesCount > 0) {
        console.log(colorize('yellow', `📏 ${largeFilesCount} files are large (>200 lines)`));
    }
    
    console.log(colorize('green', '\n✅ Recommendations:'));
    console.log('   • Keep cyclomatic complexity below 10');
    console.log('   • Maintain files under 200 lines');
    console.log('   • Aim for maintainability index above 70');
    console.log('   • Consider refactoring complex functions');
    console.log('   • Add unit tests for complex logic');
    
    header('End of Report');
}

// Run ESLint analysis
function runESLintAnalysis() {
    subheader('ESLint Code Quality Analysis');
    
    try {
        const output = execSync(`npx eslint src --config .eslintrc.metrics.json --format json`, { 
            encoding: 'utf8',
            cwd: path.join(__dirname, '..')
        });
        
        const results = JSON.parse(output);
        const totalIssues = results.reduce((sum, file) => sum + file.messages.length, 0);
        const errorCount = results.reduce((sum, file) => sum + file.errorCount, 0);
        const warningCount = results.reduce((sum, file) => sum + file.warningCount, 0);
        
        console.log(`${colorize('red', '🚨 Total Issues:')} ${totalIssues}`);
        console.log(`${colorize('red', '❌ Errors:')} ${errorCount}`);
        console.log(`${colorize('yellow', '⚠️  Warnings:')} ${warningCount}`);
        
        // Group issues by rule
        const issuesByRule = {};
        results.forEach(file => {
            file.messages.forEach(message => {
                if (!issuesByRule[message.ruleId]) {
                    issuesByRule[message.ruleId] = 0;
                }
                issuesByRule[message.ruleId]++;
            });
        });
        
        const topIssues = Object.entries(issuesByRule)
            .sort(([,a], [,b]) => b - a)
            .slice(0, 5);
            
        if (topIssues.length > 0) {
            console.log(colorize('yellow', '\n🔍 Top Issues by Rule:'));
            topIssues.forEach(([rule, count], index) => {
                console.log(`  ${index + 1}. ${rule}: ${count} occurrences`);
            });
        }
        
    } catch (error) {
        console.log(colorize('yellow', '⚠️  ESLint analysis could not be completed'));
        console.log(colorize('red', error.message));
    }
}

// Run the complete analysis
try {
    generateReport();
    runESLintAnalysis();
} catch (error) {
    console.error(colorize('red', 'Error generating report:'), error.message);
    process.exit(1);
}
