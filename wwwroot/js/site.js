// Auto-dismiss alerts after 4s
document.querySelectorAll('.alert-dismissible').forEach(a => {
    setTimeout(() => { const b = bootstrap.Alert.getOrCreateInstance(a); b?.close(); }, 4000);
});

// Confirm delete on any [data-confirm] form
document.querySelectorAll('form[data-confirm]').forEach(f => {
    f.addEventListener('submit', e => {
        if (!confirm(f.dataset.confirm)) e.preventDefault();
    });
});
