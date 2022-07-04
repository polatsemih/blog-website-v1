// RESPONSIVE MENU
const menuOpen = document.querySelector('.menu-open');
const menuClose = document.querySelector('.menu-close');
const navMenu = document.querySelector('.nav-menu');

menuOpen.addEventListener('click', () => {
    menuOpen.classList.remove('active');
    menuClose.classList.add('active');
    navMenu.classList.add('active');
});

menuClose.addEventListener('click', () => {
    menuClose.classList.remove('active');
    menuOpen.classList.add('active');
    navMenu.classList.remove('active');
});