import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import fs from 'fs';

const defaultDirName = path.resolve(__dirname, '../wwwroot');

function fixPartialFilePath(filePath) {
    return filePath.replace("//", "/").replace("\\", "/").replace("\\\\", "/");
}

function getAllFiles(dir, fileList = []) {
    // If directory doesn't exist, return empty array
    if (!fs.existsSync(dir)) {
        return [];
    }

    // Read all files and directories in the given directory
    fs.readdirSync(dir).forEach(file => {
        const fullPath = path.resolve(dir, file);

        if (fs.statSync(fullPath).isDirectory()) {
            getAllFiles(fullPath, fileList);
        } else {
            if (/\.(js|ts|jsx|tsx|css)$/.test(fullPath)) {
                fileList.push(fullPath);
            }
        }
    });

    return fileList;
}

function getInputFiles(rootFolders = []) {
    const inputFiles = {};

    if (rootFolders.length > 0) {
        for (const rootFolder of rootFolders) {
            const rootFolderDir = path.resolve(defaultDirName, rootFolder);

            getAllFiles(rootFolderDir).forEach(file => {
                inputFiles[fixPartialFilePath(path.relative(defaultDirName, file))] = fixPartialFilePath(file);
            });
        }
    }

    return inputFiles;
}

export default defineConfig(({ command }) => ({
    base: command === 'build' ? '/dist/' : '/',
    plugins: [vue()],
    build: {
        outDir: '../wwwroot/dist',
        manifest: true,
        emptyOutDir: true,
        rollupOptions: {
            input: getInputFiles(["js", "css"])
        }
    },
}));