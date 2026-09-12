import http from 'node:http';
import { readFile } from 'node:fs/promises';

const root = new URL('./site/', import.meta.url);
const port = Number(process.argv[2] ?? 4173);
if (!Number.isInteger(port) || port < 1 || port > 65535) {
  throw new Error('Pass a port between 1 and 65535.');
}

const assets = new Map([
  ['index.html', 'text/html; charset=utf-8'],
  ['style.css', 'text/css; charset=utf-8'],
  ['pages.css', 'text/css; charset=utf-8'],
  ['app.mjs', 'text/javascript; charset=utf-8'],
  ['engine.mjs', 'text/javascript; charset=utf-8'],
  ['i18n.mjs', 'text/javascript; charset=utf-8'],
  ['theme-init.js', 'text/javascript; charset=utf-8'],
  ['favicon.svg', 'image/svg+xml'],
  ['EasyParsing-LICENSE.txt', 'text/plain; charset=utf-8'],
]);

const server = http.createServer(async (request, response) => {
  if (request.method !== 'GET' && request.method !== 'HEAD') {
    response.writeHead(405, { Allow: 'GET, HEAD' }).end();
    return;
  }
  let name;
  try {
    const pathname = new URL(request.url, 'http://localhost').pathname;
    name = pathname === '/' ? 'index.html' : decodeURIComponent(pathname.slice(1));
  } catch {
    response.writeHead(400).end('Bad request');
    return;
  }
  if (!assets.has(name)) {
    response.writeHead(404).end('Not found');
    return;
  }
  try {
    const body = await readFile(new URL(name, root));
    response.writeHead(200, {
      'Content-Type': assets.get(name),
      'Content-Length': body.length,
      'Cache-Control': 'no-store',
      'X-Content-Type-Options': 'nosniff',
    });
    response.end(request.method === 'HEAD' ? undefined : body);
  } catch {
    response.writeHead(500).end('Could not read the site asset.');
  }
});

server.on('error', error => {
  console.error(`Could not start the preview: ${error.message}`);
  process.exitCode = 1;
});
server.listen(port, '127.0.0.1', () => {
  console.log(`EasyParsing Lab: http://127.0.0.1:${port}`);
});
