import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import fs from 'fs';

const partialDirName = '../wwwroot';
const partialDirPath = path.resolve(__dirname, partialDirName);

function getInputFiles(rootFolders = []) {
    function normalizeUrl(url) {
        return url
            .replace(/\\+/g, '/')
            .replace(/\/+/g, '/')
            .replace(/^\/|\/$/g, '')
            .replace(partialDirName, '');
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

    const inputFiles = {};

    if (rootFolders.length > 0) {
        for (const rootFolder of rootFolders) {
            const rootFolderDir = path.resolve(partialDirPath, rootFolder);

            getAllFiles(rootFolderDir).forEach(file => {
                inputFiles[normalizeUrl(path.relative(partialDirPath, file))] = normalizeUrl(file);
            });
        }
    }

    return inputFiles;
}

function modifyManifestKeys() {
    return {
        name: 'modify-manifest-keys',
        apply: 'build',
        closeBundle() {
            const manifestPath = path.resolve(partialDirPath, 'dist/.vite/manifest.json');

            if (fs.existsSync(manifestPath)) {
                const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf-8'));

                const updatedManifest = {};
                for (const key in manifest) {

                    // Remove "../wwwroot" from the key
                    if (manifest.hasOwnProperty(key)) {
                        const updatedKey = key.replace(/^(\.\.\/wwwroot)\//, '');
                        updatedManifest[updatedKey] = manifest[key];
                    }
                }

                // Write the modified manifest back to the file
                fs.writeFileSync(manifestPath, JSON.stringify(updatedManifest, null, 2));
            }
        },
    };
}

export default defineConfig(({ command }) => ({
    base: command === 'build' ? '/dist/' : '/',
    build: {
        outDir: '../wwwroot/dist',
        manifest: true,
        emptyOutDir: true,
        rollupOptions: {
            input: getInputFiles(["js", "css"])
        }
    },
    plugins: [vue(), modifyManifestKeys()],
}));