const confirmContainer = document.querySelector('.confirm-container');

document.querySelector('#button-form').addEventListener('click', (e) => {
    e.stopImmediatePropagation();
    e.preventDefault();
    confirmContainer.classList.add("active");
});

document.querySelector('#button-confirm').addEventListener('click', () => {
    document.querySelector('#form-confirm').submit();
    confirmContainer.classList.remove("active");
});

document.querySelector('#button-cancel').addEventListener('click', () => {
    confirmContainer.classList.remove("active");
});