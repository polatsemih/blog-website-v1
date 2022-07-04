const buttonScrollToTop = document.querySelector('.button-scrollToTop');

window.addEventListener('scroll', () => {
    buttonScrollToTop.classList.toggle("active", window.scrollY > 500);
});

buttonScrollToTop.addEventListener('click', () => {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
});