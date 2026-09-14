import assert from 'node:assert/strict';
import { readFile, access } from 'node:fs/promises';
import { spawnSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import vm from 'node:vm';
import { traceParse, examples, valueText } from './site/engine.mjs';
import { initialLanguage, translate } from './site/i18n.mjs';

const root = new URL('./site/', import.meta.url);
const html = await readFile(new URL('index.html', root), 'utf8');
for (const match of html.matchAll(/(?:src|href)="\.\/([^"#]+)"/g)) {
  await access(new URL(match[1], root));
}
for (const file of ['app.mjs', 'engine.mjs', 'i18n.mjs', 'theme-init.js']) {
  const result = spawnSync(process.execPath, ['--check', fileURLToPath(new URL(file, root))]);
  assert.equal(result.status, 0, result.stderr.toString());
}

const decimal = traceParse('123.456', 'decimal');
assert.deepEqual(decimal.result, {
  ok: true, pos: 7, value: { type: 'JsonDecimalValue', value: '123.456' },
});
const choice = traceParse('123', 'choice');
assert.equal(choice.result.value.type, 'JsonLongValue');
assert.equal(choice.result.pos, 3);
assert(choice.events.some(event => event.type === 'backtrack' && event.from === 3 && event.pos === 0));
assert.equal(traceParse('123x456', 'decimal').result.pos, 3);
assert.equal(traceParse('123x456', 'decimal').result.ok, false);
assert.equal(traceParse('123.xyz', 'choice').result.pos, 3);
assert.equal(traceParse('abc', 'choice').result.ok, false);
assert.equal(valueText(traceParse('FALSE', 'json').result.value), 'false');
assert.equal(traceParse('[]', 'json').result.ok, false);
assert.equal(traceParse('[1]', 'json').result.ok, false);
assert.equal(traceParse('[1,2]', 'json').result.ok, true);
assert.equal(traceParse('[[],2]', 'json').result.exception, true);
assert.equal(traceParse('{"a":1,"a":2}', 'json').result.exception, true);
assert.equal(traceParse('9223372036854775808', 'json').result.exception, true);
assert.equal(traceParse('9'.repeat(801), 'decimal').result.limited, true);
assert.equal(traceParse('{"x":'.repeat(60), 'json').result.limited, true);

let traces = 0;
for (const [mode, inputs] of Object.entries(examples)) {
  for (const [, input] of inputs) {
    for (let length = 0; length <= input.length; length++) {
      const trace = traceParse(input.slice(0, length), mode);
      traces++;
      for (const event of trace.events) {
        assert(event.pos >= 0 && event.pos <= length);
        if (event.type === 'backtrack') assert.equal(event.pos, trace.nodes[event.node].start);
        assert.notEqual(translate(event.message, 'en'), event.message, `Untranslated: ${event.message}`);
        assert.equal(translate(event.message, 'fr'), event.message);
      }
    }
  }
}

assert.equal(initialLanguage(null), 'en');
assert.equal(initialLanguage('fr'), 'fr');
assert.equal(translate('JsonDecimalValueParser', 'en'), 'JsonDecimalValueParser');
assert.equal(translate('Le prédicat attend « true » ; « échec » a été lu.', 'en'),
  'The predicate expects “true”; “échec” was read.');

const themeScript = await readFile(new URL('theme-init.js', root), 'utf8');
for (const [saved, system, expected] of [
  [null, false, 'light'], [null, true, 'dark'], ['light', true, 'light'], ['dark', false, 'dark'],
]) {
  const document = { documentElement: { dataset: {} } };
  vm.runInNewContext(themeScript, {
    document, localStorage: { getItem: () => saved },
    window: { matchMedia: () => ({ matches: system }) },
  });
  assert.equal(document.documentElement.dataset.theme, expected);
}
const document = { documentElement: { dataset: {} } };
vm.runInNewContext(themeScript, {
  document, localStorage: { getItem() { throw new Error('Storage unavailable'); } },
  window: { matchMedia: () => ({ matches: true }) },
});
assert.equal(document.documentElement.dataset.theme, 'dark');
console.log(`Site checks passed: assets, syntax, parser cases, ${traces} traces, FR/EN, and themes.`);
