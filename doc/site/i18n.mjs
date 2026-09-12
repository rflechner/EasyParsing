// UI translations only. User input, captured values and C# identifiers stay intact.
export const initialLanguage=value=>value==='fr'?'fr':'en';
export let language='en';
try { language=initialLanguage(localStorage.getItem('easyparsing-language')); } catch {}
export function setLanguage(value){language=value==='en'?'en':'fr';try{localStorage.setItem('easyparsing-language',language)}catch{}}
const pairs = [
['Navigation principale','Main navigation'],['Utiliser','Use it'],['Comprendre','Understand it'],['Mode sombre','Dark mode'],['Mode clair','Light mode'],['Activer le mode sombre','Enable dark mode'],['Activer le mode clair','Enable light mode'],
['DÉMARRER AVEC EASYPARSING','GET STARTED WITH EASYPARSING'],['Construisez vos parseurs.','Build your parsers.'],['Une brique à la fois.','One piece at a time.'],
['Un parser combinator assemble de petits parseurs pour en construire un plus grand. Décrivez les éléments de votre grammaire, puis reliez-les avec les opérateurs et la syntaxe LINQ de C#.','A parser combinator combines small parsers to build a larger one. Describe the pieces of your grammar, then connect them with C# operators and LINQ syntax.'],
['INSTALLATION','INSTALLATION'],['Ajouter le package NuGet','Add the NuGet package'],['Dans le dossier de votre projet .NET, après avoir configuré l’authentification GitHub Packages :','From your .NET project folder, after configuring GitHub Packages authentication:'],
['Copier','Copy'],['Copié !','Copied!'],['Copie indisponible : sélectionnez la commande.','Copy unavailable: select the command.'],['Copier la commande NuGet','Copy the NuGet command'],['Copier l’exemple C#','Copy the C# example'],
['Le README utilise le flux GitHub Packages de rflechner.','The README uses rflechner’s GitHub Packages feed.'],['Configurer l’authentification ↗','Configure authentication ↗'],['Instructions du dépôt ↗','Repository instructions ↗'],
['Un compte GitHub et un jeton personnel classique disposant du droit read:packages sont nécessaires. Configurez-les dans votre client NuGet à l’aide du guide officiel.','A GitHub account and a classic personal access token with read:packages are required. Configure them in your NuGet client using the official guide.'],
['DE PETITS PARSEURS À UN RÉSULTAT','FROM SMALL PARSERS TO ONE RESULT'],['Une séquence qui lit 123.456','A sequence that reads 123.456'],['chiffres','digits'],['point','dot'],['valeur décimale','decimal value'],
['Chaque parseur reçoit un contexte : la position courante et le texte restant. Il renvoie un succès ou un échec, une valeur en cas de succès et un contexte de sortie.','Each parser receives a context: the current position and the remaining input. It returns success or failure, a value on success, and an output context.'],
['VOTRE PREMIER PARSEUR','YOUR FIRST PARSER'],['Du texte à une valeur C#','From text to a C# value'],['Ce code utilise uniquement le package EasyParsing.','This code only uses the EasyParsing package.'],['Sortie','Output'],
['Lire','Play'],['1. Reconnaître','1. Recognize'],['Les deux ManySatisfy capturent les chiffres ; OneCharText reconnaît le point.','The two ManySatisfy parsers capture the digits; OneCharText recognizes the dot.'],
['2. Enchaîner','2. Sequence'],['Chaque from poursuit la lecture avec le contexte du parseur précédent. Un échec arrête toute la séquence.','Each from continues with the previous parser’s context. A failure stops the whole sequence.'],
['3. Transformer','3. Transform'],['Le select transforme les captures en decimal. Votre grammaire devient un parseur réutilisable.','The select transforms the captures into a decimal. Your grammar becomes a reusable parser.'],
['Le parsing peut réussir sur un préfixe. Pour exiger une lecture complète, vérifiez aussi result.Context.Remaining.IsEmpty.','Parsing may succeed on a prefix. To require a complete parse, also check result.Context.Remaining.IsEmpty.'],
['Et si plusieurs formes sont possibles ?','What if several forms are possible?'],['Avec A | B, EasyParsing essaie A en premier. Si A échoue, B reçoit le même contexte de départ. C’est ce qui permet le backtracking.','With A | B, EasyParsing tries A first. If A fails, B receives the same starting context. This enables backtracking.'],
['Voir le parsing en action','See parsing in action'],['Explorer le pipeline, les alternatives et le JSON.','Explore the pipeline, alternatives and JSON.'],
['from petitsParseurs','from smallParsers'],['select grandesIdées','select bigIdeas'],['0,5×','0.5×'],
['EasyParsing, accueil','EasyParsing, home'],['Langue','Language'],
['Le parsing,','Parsing,'],['pas à pas.','step by step.'],['Une chaîne. Des petits parseurs.','One string. Small parsers.'],['Chaque décision devient visible.','Every decision becomes visible.'],
['Pipeline décimal','Decimal pipeline'],['Parseur JSON','JSON parser'],['Démonstrations','Demos'],['Chaîne à parser','Input string'],['Analyser','Parse'],['Essayer','Try'],['Ctrl + Entrée pour analyser','Ctrl + Enter to parse'],
['Le texte a changé. Lancez l’analyse pour actualiser la visualisation.','The input has changed. Parse it to update the visualization.'],
['RUBAN D’ENTRÉE','INPUT TAPE'],['Caractères de la chaîne et position du curseur','Input characters and cursor position'],['Consommé','Consumed'],['Curseur','Cursor'],['Retour arrière','Backtrack'],['∅ = fin du texte','∅ = end of input'],
['COMPOSITION SÉQUENTIELLE','SEQUENTIAL COMPOSITION'],['Trois parseurs, un décimal','Three parsers, one decimal'],['CHOIX ORDONNÉ · OPÉRATEUR |','ORDERED CHOICE · OPERATOR |'],['Échouer. Revenir. Réessayer.','Fail. Rewind. Try again.'],['COMPOSITION RÉCURSIVE','RECURSIVE COMPOSITION'],['Un parseur fait appel aux autres','One parser calls the others'],
['Vue','View'],['Schéma','Diagram'],['Appels','Calls'],['En attente','Waiting'],['Actif','Active'],['Succès','Success'],['Échec','Failure'],['À CETTE ÉTAPE','AT THIS STEP'],['Contexte initial','Initial context'],['VARIABLES CAPTURÉES','CAPTURED VARIABLES'],['RÉSULTAT','RESULT'],['EN ATTENTE','WAITING'],
['Revenir au début','Go to the beginning'],['Étape précédente','Previous step'],['Étape précédente (←)','Previous step (←)'],['Étape suivante','Next step'],['Étape suivante (→)','Next step (→)'],['Aller au résultat','Go to the result'],['Lire','Play'],['Pause','Pause'],['Progression','Progress'],['Étape de la trace','Trace step'],['Aller au prochain retour arrière','Go to the next backtrack'],['↶ Prochain retour','↶ Next backtrack'],['Vitesse','Speed'],['Espace : lecture · ← → : pas à pas','Space: play · ← →: step by step'],
['À propos de la fidélité au code EasyParsing','About fidelity to the EasyParsing code'],['Cette visualisation exécute une adaptation JavaScript instrumentée du code C# de la branche','This visualization runs an instrumented JavaScript adaptation of the C# code on the'],[', consulté le 12 septembre 2026. Elle ne charge pas la bibliothèque .NET. Les enveloppes', 'branch, accessed on September 12, 2026. It does not load the .NET library. The'],['et les alternatives imbriquées sont regroupées pour rendre les étapes lisibles.','wrappers and nested alternatives are grouped to make the steps readable.'],
['L’exemple JSON accepte les guillemets simples et les booléens sans distinction de casse. Il ne gère pas','The JSON sample accepts single quotes and case-insensitive booleans. It does not handle'],[', les nombres négatifs ou les exposants ; il ne vérifie pas que tout le texte est consommé. Aucun espace initial n’est ajouté ou supprimé par la démo.',', negative numbers or exponents; it does not check that all input is consumed. The demo does not add or remove leading whitespace.'],
['Le comportement actuel de','The current behavior of'],['est conservé : les collections vides et les collections d’un seul élément échouent. Les échappements des chaînes suivent','is preserved: empty and single-item collections fail. String escapes follow'],[', et non le standard JSON complet. Une exception de conversion arrête le parsing, sans essayer l’alternative suivante.',', rather than the full JSON standard. An exception stops parsing without trying the next alternative.'],
['Les tableaux directement imbriqués déclenchent une','Directly nested arrays raise a'],['dans cette version : leur alternative capture','in this version: their alternative captures'],['avant son initialisation. Une exception arrête le parsing, sans essayer l’alternative suivante.','before it is initialized. An exception stops parsing without trying the next alternative.'],
['Limites de la visualisation : 800 caractères, 14 000 étapes, profondeur limitée et décimaux jusqu’à 28 chiffres significatifs / 28 décimales. Les valeurs numériques sont affichées sans passer par les nombres flottants JavaScript.','Visualization limits: 800 characters, 14,000 steps, limited depth, and decimals up to 28 significant digits / 28 decimal places. Numeric values are displayed without using JavaScript floating-point numbers.'],
['Consulter JsonParser.cs ↗','View JsonParser.cs ↗'],['Licence MIT','MIT License'],['Parsing en mouvement','Parsing in motion'],['Parsing terminé','Parsing complete'],['Appel sélectionné','Selected call'],['Suivre la lecture','Follow playback'],['Les captures de l’appel courant apparaîtront ici.','Captures from the current call will appear here.'],['La valeur construite apparaîtra à la fin de la lecture.','The constructed value will appear at the end of playback.'],['Tout le texte a été consommé.','All input has been consumed.'],['Aucune alternative ne correspond.','No alternative matches.'],
['PRÊT','READY'],['ENTRÉE','ENTER'],['LECTURE','READ'],['RÉPÉTITION','REPEAT'],['SUCCÈS','SUCCESS'],['ÉCHEC','FAILURE'],['ALTERNATIVE','ALTERNATIVE'],['RETOUR','REWIND'],['TERMINÉ','FINISHED'],['EXCEPTION','EXCEPTION'],['LIMITE','LIMIT'],['PRÉFIXE','PREFIX'],['INSPECTION','INSPECTION'],
['Décimal','Decimal'],['Point manquant','Missing dot'],['Fraction absente','Missing fraction'],['Texte restant','Remaining text'],['Préfixe accepté','Accepted prefix'],['Aucune branche','No match'],['Objet','Object'],['Récursion','Recursion'],['Booléen','Boolean'],['Erreur','Error'],['Liste vide','Empty list'],
['Tout est lié.','Everything is connected.'],['Si un parseur échoue, le parseur composé échoue.','If a parser fails, the combined parser fails.'],['Mapping des captures','Map captured values'],['✓ Une valeur composée','✓ One combined value'],['× La séquence s’arrête ici','× The sequence stops here'],['↓ une seule valeur en sortie','↓ a single output value'],['contexte ↓','context ↓'],['Décimal | Entier','Decimal | Integer'],['01 · PREMIER ESSAI','01 · FIRST ATTEMPT'],['02 · SI ÉCHEC','02 · ON FAILURE'],['Si le point est absent, l’entier reprend les chiffres depuis la position 0.','If the dot is missing, the integer reads the digits again from position 0.'],['· le contexte de départ est réutilisé.','· the original context is reused.'],
['Lancez la lecture pour voir les appels s’emboîter.','Start playback to see how calls nest.'],['La fenêtre suit l’appel courant ; les ancêtres restent visibles.','The window follows the current call; ancestors remain visible.'],['Les alternatives sont essayées de gauche à droite.','Alternatives are tried from left to right.'],['Choix ordonné','Ordered choice'],['01 · Chaîne','01 · String'],['02 · Booléen','02 · Boolean'],['03 · Décimal','03 · Decimal'],['04 · Entier','04 · Integer'],['05 · Objet','05 · Object'],['06 · Tableau','06 · Array'],['APPELS IMBRIQUÉS','NESTED CALLS'],['↳ récursion de ValueParser','↳ ValueParser recursion'],['Pile des appels','Call stack'],['Extrait pédagogique : choix + OrElseParser','Teaching excerpt: choice + OrElseParser'],['Extrait de JsonParser.cs · develop','Excerpt from JsonParser.cs · develop'],
['Chaque','Each'],['transmet le contexte au parseur suivant. Le','passes the context to the next parser. The'],['transforme les captures en une valeur. Si un composant échoue, la séquence échoue.','transforms captures into a value. If one component fails, the sequence fails.'],['L’opérateur','The'],['essaie les alternatives dans l’ordre. Chacune reçoit le','operator tries alternatives in order. Each receives the'],['contexte initial','original context'],[': les caractères lus par une branche qui échoue sont disponibles pour la suivante.',': characters consumed by a failing branch remain available to the next one.'],['essaie chaîne → booléen → décimal → entier → objet → tableau. Les objets et tableaux rappellent','tries string → boolean → decimal → integer → object → array. Objects and arrays call'],['pour lire leurs valeurs.','to read their values.'],
['Between(ouverture, contenu, fermeture)','Between(opening, content, closing)'],['SkipSpaces() >> fermeture','SkipSpaces() >> closing'],
['Le contexte initial est créé à la position 0.','The initial context is created at position 0.'],['Fin du texte : la répétition s’arrête.','End of input: repetition stops.'],['Le contenu entre guillemets est capturé.','The content between the quotes is captured.'],['Succès : tout le texte a été consommé.','Success: all input has been consumed.'],['Succès du parseur : une partie du texte reste à lire.','Parser success: some input remains unread.'],['Le parseur composé échoue.','The combined parser fails.'],['Trace limitée à 14 000 étapes. Réduisez le texte.','The trace is limited to 14,000 steps. Use a shorter input.'],['Profondeur maximale de cette démonstration atteinte.','The maximum depth of this demo has been reached.'],['La démonstration accepte au maximum 800 caractères.','The demo accepts up to 800 characters.'],['La démo ne simule pas les arrondis et dépassements de System.Decimal au-delà de 28 chiffres.','The demo does not simulate System.Decimal rounding or overflow beyond 28 digits.'],['FormatException : decimal.Parse ne convertit pas ces chiffres Unicode.','FormatException: decimal.Parse cannot convert these Unicode digits.'],['FormatException : long.Parse ne convertit pas ces chiffres Unicode.','FormatException: long.Parse cannot convert these Unicode digits.'],['OverflowException : la valeur dépasse Int64.MaxValue.','OverflowException: the value exceeds Int64.MaxValue.'],['NullReferenceException : le tableau imbriqué utilise ItemsParser avant son initialisation dans cette version C#.','NullReferenceException: the nested array uses ItemsParser before it is initialized in this C# version.'],
];
const dictionary=new Map(pairs);
const rules=[
 [/^(\d+) \/ 800 caractères$/,(_,n)=>`${n} / 800 characters`],
 [/^Étape (\d+) sur (\d+)$/,(_,a,b)=>`Step ${a} of ${b}`],
 [/^(\d+) caractère\(s\) restant\(s\)\. Le parseur n’impose pas la fin du texte\.$/,(_,n)=>`${n} character(s) remain. The parser does not require the end of input.`],
 [/^Cet appel est terminé : (succès|échec), position (\d+) → (\d+)\.$/,(_,s,a,b)=>`This call has finished: ${s==='succès'?'success':'failure'}, position ${a} → ${b}.`],
 [/^Cet appel a commencé à la position (\d+)\.$/,(_,n)=>`This call started at position ${n}.`],
 [/^Entrée dans (.+)\.$/,(_,s)=>`Entering ${translate(s,'en')}.`],
 [/^(.+) réussit\.$/,(_,s)=>`${translate(s,'en')} succeeds.`],
 [/^Le contexte de (.+) revient de (\d+) à (\d+)\.$/,(_,s,a,b)=>`The context of ${translate(s,'en')} returns from ${a} to ${b}.`],
 [/^« ([\s\S]*) » satisfait le prédicat\.$/,(_,s)=>`“${s}” satisfies the predicate.`],
 [/^Aucun caractère ne satisfait (.+)\.$/,(_,s)=>`No character satisfies ${s}.`],
 [/^« ([\s\S]*) » ne satisfait plus le prédicat : la répétition s’arrête avec succès\.$/,(_,s)=>`“${s}” no longer satisfies the predicate: repetition ends successfully.`],
 [/^« ([\s\S]*) » est consommé\.$/,(_,s)=>`“${s}” is consumed.`],
 [/^« ([\s\S]*) » attendu à la position (\d+) ; fin du texte\.$/,(_,s,n)=>`Expected “${s}” at position ${n}; end of input.`],
 [/^« ([\s\S]*) » attendu à la position (\d+) ; « ([\s\S]*) » rencontré\.$/,(_,s,n,c)=>`Expected “${s}” at position ${n}; found “${c}”.`],
 [/^(\d+) caractère\(s\) d’espacement ignoré\(s\)\.$/,(_,n)=>`${n} whitespace character(s) skipped.`],
 [/^Retour arrière (\d+) → (\d+)\. L’alternative suivante reprend le contexte initial\.$/,(_,a,b)=>`Backtrack ${a} → ${b}. The next alternative receives the original context.`],
 [/^Alternative suivante, au même point de départ : (\d+)\.$/,(_,n)=>`Next alternative, at the same starting position: ${n}.`],
 [/^Guillemet (.+) fermant introuvable\.$/,(_,s)=>`Closing quote ${s} not found.`],
 [/^Le prédicat attend « (.+) » ; « ([\s\S]*) » a été lu\.$/,(_,a,b)=>`The predicate expects “${a}”; “${b}” was read.`],
 [/^ArgumentException : clé « ([\s\S]*) » dupliquée dans ToDictionary\.$/,(_,s)=>`ArgumentException: duplicate key “${s}” in ToDictionary.`],
 [/^(.+) : (en attente|actif|succès|échec)$/,(_,a,b)=>`${translate(a,'en')}: ${{'en attente':'waiting',actif:'active','succès':'success','échec':'failure'}[b]}`],
 [/^Position (\d+) : ([\s\S]*?)(, curseur)?$/,(_,n,s,c)=>`Position ${n}: ${s==='espace'?'space':s}${c?', cursor':''}`],
];
export function translate(source, locale=language){
  if(locale==='fr')return source;
  const text=source.trim();
  let target=dictionary.get(text);
  if(target===undefined&&text.includes(' · ')&&(/attendu|satisfait/.test(text)))target=text.split(' · ').map(s=>translate(s,locale)).join(' · ');
  if(target===undefined)for(const [regex,replace] of rules){if(regex.test(text)){target=text.replace(regex,replace);break;}}
  return target===undefined?source:source.slice(0,source.indexOf(text))+target+source.slice(source.indexOf(text)+text.length);
}
const originals=new WeakMap();
const attributes=new WeakMap();
export function localize(root=document.body){
  const walker=document.createTreeWalker(root,NodeFilter.SHOW_TEXT);
  while(walker.nextNode()){
    const node=walker.currentNode;
    if(!node.textContent.trim()||node.parentElement.closest('script,style,textarea,pre,code,.node-value,.letter,.brand,.capture-row'))continue;
    let record=originals.get(node);
    if(!record||record.last!==node.textContent)record={source:node.textContent};
    const result=translate(record.source);node.textContent=result;record.last=result;originals.set(node,record);
  }
  for(const el of root.querySelectorAll('[aria-label],[title],[aria-valuetext]')){
    let records=attributes.get(el)??{};
    for(const name of ['aria-label','title','aria-valuetext']){
      if(!el.hasAttribute(name))continue;
      const value=el.getAttribute(name);let r=records[name];
      if(!r||r.last!==value)r={source:value};
      r.last=translate(r.source);el.setAttribute(name,r.last);records[name]=r;
    }
    attributes.set(el,records);
  }
  document.documentElement.lang=language;
  document.title=language==='fr'?'EasyParsing — Le parsing, pas à pas':'EasyParsing — Parsing, step by step';
  document.querySelector('meta[name="description"]').content=language==='fr'?'Explorez EasyParsing : parser combinators, contexte et backtracking animés, du nombre décimal au parseur JSON récursif.':'Explore EasyParsing: animated parser combinators, context and backtracking, from decimal numbers to a recursive JSON parser.';
  document.querySelectorAll('[data-lang]').forEach(b=>b.setAttribute('aria-pressed',b.dataset.lang===language));
}
