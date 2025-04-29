document.addEventListener('DOMContentLoaded', () => {
    const canvas = document.getElementById('steamCanvas');
    const ctx = canvas.getContext('2d');

    // Настройки
    const fadeStartHeight = 0.25 * canvas.height; // 25% от верха (исчезает до 75%)
    const fadeEndHeight = 0; // Верх canvas (0%)

    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    // Частицы пара
    const particles = [];
    const maxParticles = 50;

    class Particle {
        constructor() {
            this.reset();
        }

        reset() {
            this.x = Math.random() * canvas.width;
            this.y = canvas.height + Math.random() * 100;
            this.size = 5 + Math.random() * 15;
            this.speedY = -1 - Math.random() * 1;
            this.speedX = Math.random() * 0.5 - 0.25;
            this.opacity = 0.1 + Math.random() * 0.3;
        }

        update() {
            this.y += this.speedY;
            this.x += this.speedX;

            // Плавное исчезновение при приближении к 75% высоты
            if (this.y < fadeStartHeight) {
                const fadeProgress = (this.y - fadeEndHeight) / (fadeStartHeight - fadeEndHeight);
                this.opacity = Math.max(0, fadeProgress * 0.3);
            }

            // Если частица ушла за верх экрана — пересоздаём
            if (this.y < -10 || this.opacity <= 0) {
                this.reset();
            }
        }

        draw() {
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(255, 255, 255, ${this.opacity})`;
            ctx.fill();
        }
    }

    // Инициализация частиц
    for (let i = 0; i < maxParticles; i++) {
        particles.push(new Particle());
    }

    function animate() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        particles.forEach(particle => {
            particle.update();
            particle.draw();
        });

        requestAnimationFrame(animate);
    }

    animate();
});