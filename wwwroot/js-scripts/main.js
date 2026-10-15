const hamburgerBtn = document.querySelector('.hamburger');
const smallNav = document.querySelector('.small-nav');

hamburgerBtn.addEventListener('click', () => {
    smallNav.classList.toggle('open');
});