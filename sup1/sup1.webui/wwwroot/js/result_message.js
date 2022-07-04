const alertContainer = document.querySelector('.alert-container');
const alertCloseIcon = document.querySelector('.alert-close-icon');

alertCloseIcon.addEventListener('click', () => {
    alertContainer.style.right = "-100%";
    setTimeout(() => {
        alertContainer.classList.add('disable');
    }, 2500);
});