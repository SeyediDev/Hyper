(() => {
    const toggle = document.querySelector('.mobile-toggle');
    const nav = document.getElementById('admin-navigation');
    if (!toggle || !nav) return;
    const setOpen = open => {
        document.body.classList.toggle('nav-open', open);
        toggle.setAttribute('aria-expanded', String(open));
        nav.inert = window.innerWidth <= 760 && !open;
    };
    toggle.addEventListener('click', () => setOpen(toggle.getAttribute('aria-expanded') !== 'true'));
    document.addEventListener('keydown', event => {
        if (event.key === 'Escape' && toggle.getAttribute('aria-expanded') === 'true') { setOpen(false); toggle.focus(); }
    });
    document.addEventListener('click', event => { if (!nav.contains(event.target) && !toggle.contains(event.target)) setOpen(false); });
    window.addEventListener('resize', () => setOpen(false));
    setOpen(false);
})();
