// Instrumented browser adaptation of rflechner/EasyParsing (MIT).
// Infrastructure wrappers (Cast, AsString and nested OrElse) are folded in the trace.
export const SOURCE = 'https://github.com/rflechner/EasyParsing/blob/develop/src/EasyParsing.Samples.Json/JsonParser.cs';
export const examples = {
  decimal: [
    ['Décimal', '123.456'], ['Point manquant', '123x456'],
    ['Fraction absente', '123.'], ['Texte restant', '123.456abc'],
  ],
  choice: [
    ['Retour arrière', '123'], ['Décimal', '123.456'],
    ['Préfixe accepté', '123.xyz'], ['Aucune branche', 'abc'],
  ],
  json: [
    ['Objet', '{"name": "EasyParsing", "version": 2}'],
    ['Récursion', '{"name": "EasyParsing", "values": [123, 4.56, false]}'],
    ['Booléen', 'false'], ['Erreur', '{"name": "EasyParsing", "version": }'],
    ['Liste vide', '[]'],
  ],
};
export const snippets = {
  decimal: `from abs in ManySatisfy(char.IsDigit)
from point in OneCharText('.')
from rel in ManySatisfy(char.IsDigit)
select new JsonDecimalValue(
    decimal.Parse($"{abs}.{rel}",
        NumberStyles.AllowDecimalPoint,
        CultureInfo.InvariantCulture));`,
  choice: `JsonDecimalValueParser.Cast<JsonDecimalValue, JsonValue>()
    | JsonLongValueParser;

// OrElseParser.Parse(context)
foreach (var parser in parsers)
{
    var result = parser.Parse(context);
    if (result.Success && result.Result != null)
        return Success(result.Context, result.Result);
}
// Chaque alternative reçoit le même contexte.`,
  json: `ValueParser =>
    JsonStringValueParser.Cast<JsonStringValue, JsonValue>()
    | JsonBoolValueParser
    | JsonDecimalValueParser
    | JsonLongValueParser
    | JsonObjectParser
    | JsonArrayParser;

JsonBoolValueParser = TrueParser | FalseParser;

PropertyAssignParser =
    from key in QuotedTextParser >> SkipSpaces()
    from dotDot in KeyValueSeparator
    from value in ValueParser
    select new JsonProperty(key, value);

PropertiesListParser = PropertyAssignParser
    .SeparatedBy(SkipSpaces() >> OneCharText(',') >> SkipSpaces());

JsonObjectParser =
    Between(StartObject, PropertiesListParser, SkipSpaces() >> EndObject)
    .Select(i => new JsonObject(i.Item.ToDictionary(p => p.Name, p => p.Value)));

ItemsParser = ValueParser
    .SeparatedBy(SkipSpaces() >> OneCharText(',') >> SkipSpaces());

JsonArrayParser =>
    Between(StartArray, ItemsParser, SkipSpaces() >> EndArray)
    .Select(i => new JsonArray(i.Item));`,
};
const ok = (pos, value) => ({ok: true, pos, value});
const fail = (pos, message) => ({ok: false, pos, message});
const typed = (type, value) => ({type, value});
const isDigit = c => !!c && /^\p{Nd}$/u.test(c);
const isLetter = c => !!c && /^\p{L}$/u.test(c);
const isSpace = c => !!c && /^[\u0009-\u000D\u0085\p{Z}]$/u.test(c);
export function valueText(value, indent = 0) {
  if (value?.type === 'JsonObject') return '{' + value.value.map(([k,v]) => `${JSON.stringify(k)}: ${valueText(v, indent+1)}`).join(', ') + '}';
  if (value?.type === 'JsonArray') return '[' + value.value.map(v => valueText(v, indent+1)).join(', ') + ']';
  if (value?.type === 'JsonLongValue' || value?.type === 'JsonDecimalValue') return value.value;
  if (value?.type) return JSON.stringify(value.value);
  return JSON.stringify(value) ?? '—';
}
class DemoLimit extends Error {}
export function traceParse(input, mode = 'decimal') {
  const events = [], nodes = [], stack = [];
  let cursor = 0;
  function emit(type, pos, message, extra = {}) {
    if (events.length >= 14000) throw new DemoLimit('Trace limitée à 14 000 étapes. Réduisez le texte.');
    cursor = pos;
    events.push({type, pos, message, node: stack.at(-1)?.id ?? null, stack: stack.map(n=>n.id), ...extra});
  }
  function run(key, label, pos, fn, meta = {}) {
    if (stack.length > 100) throw new DemoLimit('Profondeur maximale de cette démonstration atteinte.');
    const n = {id: nodes.length, key, label, start: pos, parent: stack.at(-1)?.id ?? null, depth: stack.length, enter: events.length, ...meta};
    nodes.push(n); stack.push(n);
    emit('enter', pos, `Entrée dans ${label}.`);
    const result = fn(pos);
    if (cursor > result.pos) emit('restore', result.pos, `Le contexte de ${label} revient de ${cursor} à ${result.pos}.`, {from: cursor});
    emit(result.ok ? 'success' : 'failure', result.pos, result.ok ? `${label} réussit.` : result.message, {value: result.value, start: pos, key, capture: meta.capture});
    n.exit = events.length-1; n.result = result;
    stack.pop();
    return result;
  }
  function many(pos, predicate, label, key, capture) {
    return run(key, label, pos, p => {
      let q = p;
      while (predicate(input[q])) {
        emit('consume', q+1, `« ${input[q]} » satisfait le prédicat.`, {from: q, value: input.slice(p,q+1), capture, key}); q++;
      }
      if (q === p) return fail(p, `Aucun caractère ne satisfait ${label.includes('Digit') ? 'char.IsDigit' : 'char.IsLetter'}.`);
      emit('stop', q, q === input.length ? 'Fin du texte : la répétition s’arrête.' : `« ${input[q]} » ne satisfait plus le prédicat : la répétition s’arrête avec succès.`);
      return ok(q, input.slice(p,q));
    }, {capture});
  }
  const digits = (p,key='digits',capture) => many(p,isDigit,'ManySatisfy(char.IsDigit)',key,capture);
  function char(p, c, key='char', capture) {
    return run(key, `OneCharText('${c}')`, p, p => {
      if (input[p] !== c) return fail(p, `« ${c} » attendu à la position ${p} ; ${p === input.length ? 'fin du texte' : `« ${input[p]} » rencontré`}.`);
      emit('consume',p+1,`« ${c} » est consommé.`,{from:p,value:c,capture,key});
      return ok(p+1,c);
    },{capture});
  }
  function spaces(p) {
    return run('spaces','SkipSpaces()',p,p=>{
      let q=p; while(isSpace(input[q])) q++;
      if(q>p) emit('consume',q,`${q-p} caractère(s) d’espacement ignoré(s).`,{from:p});
      return ok(q,input.slice(p,q));
    });
  }
  function choice(p, alternatives, key, label) {
    return run(key,label,p,p=>{
      const errors=[];
      for(let i=0;i<alternatives.length;i++) {
        if(i) emit('backtrack',p,cursor>p ? `Retour arrière ${cursor} → ${p}. L’alternative suivante reprend le contexte initial.` : `Alternative suivante, au même point de départ : ${p}.`,{from:cursor,attempt:i+1});
        const r=alternatives[i](p);
        if(r.ok) return r;
        errors.push(r.message);
      }
      return fail(p,errors.join(' · '));
    });
  }
  function decimal(p) {
    return run('decimal','JsonDecimalValueParser',p,p=>{
      const a=digits(p,'abs','abs'); if(!a.ok) return a;
      const point=char(a.pos,'.','point','point'); if(!point.ok) return point;
      const b=digits(point.pos,'rel','rel'); if(!b.ok) return b;
      return run('mapDecimal','select → JsonDecimalValue',b.pos,p=>{
        if(!/^[0-9]+$/.test(a.value+b.value)) throw new Error('FormatException : decimal.Parse ne convertit pas ces chiffres Unicode.');
        const significant=(a.value+b.value).replace(/^0+/, '');
        if(significant.length>28 || b.value.length>28) throw new DemoLimit('La démo ne simule pas les arrondis et dépassements de System.Decimal au-delà de 28 chiffres.');
        return ok(p,typed('JsonDecimalValue',`${a.value.replace(/^0+(?=\d)/,'')}.${b.value}`));
      });
    });
  }
  function integer(p) {
    return run('integer','JsonLongValueParser',p,p=>{
      const r=digits(p,'intDigits','i'); if(!r.ok) return fail(p,r.message);
      return run('mapLong','select → JsonLongValue',r.pos,q=>{
        if(!/^[0-9]+$/.test(r.value)) throw new Error('FormatException : long.Parse ne convertit pas ces chiffres Unicode.');
        if(BigInt(r.value)>9223372036854775807n) throw new Error('OverflowException : la valeur dépasse Int64.MaxValue.');
        return ok(q,typed('JsonLongValue',BigInt(r.value).toString()));
      });
    });
  }
  function quoted(p) {
    function quoteBranch(c) {
      return p=>run('quote',`CreateStringParser('${c}')`,p,p=>{
        const open=char(p,c); if(!open.ok) return fail(p,open.message);
        const content=run('text','ConsumeWhile(…)',open.pos,q=>{
          let end=q;
          while(end<input.length && (input[end]!==c || (end>q && input[end-1]==='\\'))) end++;
          if(end===input.length) return fail(q,`Guillemet ${c} fermant introuvable.`);
          if(end>q) emit('consume',end,'Le contenu entre guillemets est capturé.',{from:q,value:input.slice(q,end)});
          return ok(end,input.slice(q,end).split('\\'+c).join(c));
        });
        if(!content.ok) return fail(p,content.message);
        const end=char(content.pos,c); if(!end.ok) return fail(p,end.message);
        return ok(end.pos,content.value);
      });
    }
    return choice(p,[quoteBranch("'"),quoteBranch('"')],'quoted','QuotedTextParser');
  }
  function string(p) {
    return run('string','JsonStringValueParser',p,p=>{
      const r=quoted(p); return r.ok ? ok(r.pos,typed('JsonStringValue',r.value)) : fail(p,r.message);
    });
  }
  function boolBranch(word) {
    return p=>run(word,word==='true'?'TrueParser':'FalseParser',p,p=>{
      const w=run('where',`where str.Equals("${word}")`,p,q=>{
        const r=many(q,isLetter,'ManySatisfy(char.IsLetter)','letters','str');
        if(!r.ok) return r;
        return r.value.toLowerCase()===word ? r : fail(r.pos,`Le prédicat attend « ${word} » ; « ${r.value} » a été lu.`);
      });
      return w.ok ? ok(w.pos,typed('JsonBoolValue',word==='true')) : fail(p,w.message);
    });
  }
  const bool=p=>choice(p,[boolBranch('true'),boolBranch('false')],'bool','JsonBoolValueParser');
  // >> retains the left result and resets the context on failure (Combine + Select).
  function sequence(p,parsers,label,key) {
    return run(key,label,p,p=>{
      let q=p,first;
      for(const parser of parsers) {const r=parser(q); if(!r.ok) return fail(p,r.message); if(first===undefined) first=r.value; q=r.pos;}
      return ok(q,first);
    });
  }
  const token=(p,c,name)=>sequence(p,[p=>char(p,c),spaces],name,'token');
  const separator=p=>sequence(p,[spaces,p=>char(p,','),spaces],"SkipSpaces() >> ',' >> SkipSpaces()",'separator');
  function property(p) {
    return run('property','PropertyAssignParser',p,p=>{
      const key=sequence(p,[quoted,spaces],'QuotedTextParser >> SkipSpaces()','key'); if(!key.ok)return key;
      const colon=token(key.pos,':','KeyValueSeparator'); if(!colon.ok)return colon;
      const v=value(colon.pos); if(!v.ok)return v;
      return ok(v.pos,[key.value,v.value]);
    });
  }
  // Deliberately preserves the develop implementation's empty/single-item behavior.
  function separated(p,item,label) {
    return run('separated',label,p,p=>{
      const items=[]; let current=p, r=item(current);
      do {
        if(!r.ok) return items.length ? ok(r.pos,items) : fail(current,r.message);
        const sep=separator(r.pos);
        if(!sep.ok) {
          if(!items.length) return fail(current,sep.message);
          items.push(r.value); return ok(sep.pos,items);
        }
        items.push(r.value);
        const previous=r.pos; r=item(sep.pos);
        if(!r.ok) return ok(previous,items);
        current=sep.pos;
      } while(current<input.length);
      return ok(current,items);
    });
  }
  function container(p,isObject,uninitializedItems=false) {
    const label=isObject?'JsonObjectParser':'JsonArrayParser';
    return run(isObject?'object':'array',label,p,p=>{
      const b=run('between','Between(ouverture, contenu, fermeture)',p,p=>{
        const left=token(p,isObject?'{':'[',isObject?'StartObject':'StartArray'); if(!left.ok)return fail(p,left.message);
        // ItemsParser captures ValueParser while ItemsParser is still being initialized.
        // Its eagerly built array alternative therefore holds a null middle parser.
        if(uninitializedItems) throw new Error('NullReferenceException : le tableau imbriqué utilise ItemsParser avant son initialisation dans cette version C#.');
        const middle=separated(left.pos,isObject?property:p=>value(p,true),isObject?'PropertiesListParser · SeparatedBy':'ItemsParser · SeparatedBy'); if(!middle.ok)return fail(p,middle.message);
        const right=sequence(middle.pos,[spaces,p=>token(p,isObject?'}':']',isObject?'EndObject':'EndArray')],'SkipSpaces() >> fermeture','closing'); if(!right.ok)return fail(p,right.message);
        return ok(right.pos,middle.value);
      });
      if(!b.ok)return fail(p,b.message);
      if(isObject) {
        const keys=new Set(); for(const [k] of b.value) {if(keys.has(k))throw new Error(`ArgumentException : clé « ${k} » dupliquée dans ToDictionary.`); keys.add(k);}
      }
      return ok(b.pos,typed(isObject?'JsonObject':'JsonArray',b.value));
    });
  }
  const value=(p,fromItems=false)=>choice(p,[string,bool,decimal,integer,p=>container(p,true),p=>container(p,false,fromItems)],'value','ValueParser');
  let result;
  try {
    if(input.length>800)throw new DemoLimit('La démonstration accepte au maximum 800 caractères.');
    emit('ready',0,'Le contexte initial est créé à la position 0.');
    result=mode==='decimal'?decimal(0):mode==='choice'?choice(0,[decimal,integer],'number','Décimal | Entier'):value(0);
    emit('finish',result.pos,result.ok ? (result.pos===input.length?'Succès : tout le texte a été consommé.':'Succès du parseur : une partie du texte reste à lire.') : 'Le parseur composé échoue.',{result});
  } catch(error) {
    result={ok:false,pos:cursor,message:error.message,exception:!(error instanceof DemoLimit),limited:error instanceof DemoLimit};
    // Keep the last event even when the trace limit was reached.
    events.push({type:error instanceof DemoLimit?'limit':'exception',pos:cursor,node:stack.at(-1)?.id??null,stack:stack.map(n=>n.id),message:error.message,result});
  }
  return {input,mode,events,nodes,result};
}
