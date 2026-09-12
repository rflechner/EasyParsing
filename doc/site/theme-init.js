// Apply the saved or system theme before the stylesheet paints.
(() => {
  let saved;
  try { saved = localStorage.getItem('easyparsing-theme'); } catch {}
  const dark = saved === 'dark' || (saved !== 'light' && window.matchMedia('(prefers-color-scheme: dark)').matches);
  document.documentElement.dataset.theme = dark ? 'dark' : 'light';
})();
