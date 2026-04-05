

    document.addEventListener("DOMContentLoaded", function () {
        const mainContent = document.querySelector('.main-content');
        mainContent.addEventListener('animationend', () => {
        mainContent.style.position = 'relative';
    mainContent.style.left = '0';
    mainContent.style.bottom = 'auto';
    mainContent.style.transform = 'none';
        });
    });



document.querySelectorAll('.product-card').forEach(card => {
    card.addEventListener('mouseenter', function () {
        this.style.transform = 'translateY(-8px) scale(1.02)';
    });

    card.addEventListener('mouseleave', function () {
        this.style.transform = 'translateY(0) scale(1)';
    });
});


document.querySelectorAll('.resource-tag').forEach(tag => {
    tag.addEventListener('click', function () {
        this.style.transform = 'scale(0.95)';
        setTimeout(() => {
            this.style.transform = 'scale(1)';
        }, 150);
    });
});

function animateFloatingDots() {
    const dots = document.querySelectorAll('.floating-dot');
    dots.forEach((dot, index) => {
        const randomX = Math.random() * window.innerWidth;
        const randomY = Math.random() * window.innerHeight;
        const duration = 8000 + Math.random() * 4000;

        setTimeout(() => {
            dot.style.left = randomX + 'px';
            dot.style.top = randomY + 'px';
            dot.style.transition = `all ${duration}ms ease-in-out`;
        }, index * 2000);
    });
}

setInterval(animateFloatingDots, 10000);
animateFloatingDots();

