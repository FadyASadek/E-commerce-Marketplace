
document.addEventListener('DOMContentLoaded', () => {
    
    const carousel = document.querySelector('.category-carousel');
    const container = document.querySelector('.category-container');
    const itemsContainer = document.querySelector('.category-items');
    const items = document.querySelectorAll('.category-item');
    const prevBtn = document.querySelector('.carousel-arrow.prev');
    const nextBtn = document.querySelector('.carousel-arrow.next');
    
    let currentPosition = 0;
    let itemWidth = 0;
    const gap = 24; 
    
    if (items.length > 0) {
        itemWidth = items[0].offsetWidth + gap;
    }
    
    const getMaxScroll = () => {
        const containerWidth = container.offsetWidth;
        const totalWidth = items.length * itemWidth;
        return totalWidth - containerWidth;
    };
    
    const updateCarouselPosition = () => {
        itemsContainer.style.transform = `translateX(-${currentPosition}px)`;
    };
    
    if (nextBtn) {
        nextBtn.addEventListener('click', () => {
            const maxScroll = getMaxScroll();
            currentPosition = Math.min(currentPosition + itemWidth * 3, maxScroll);
            updateCarouselPosition();
        });
    }
    
    if (prevBtn) {
        prevBtn.addEventListener('click', () => {
            currentPosition = Math.max(currentPosition - itemWidth * 3, 0);
            updateCarouselPosition();
        });
    }
    
    items.forEach(item => {
        item.addEventListener('click', function() {
            items.forEach(i => i.classList.remove('active'));
            
            this.classList.add('active');
            
            const category = this.getAttribute('data-category');
            
            const event = new CustomEvent('categoryChange', {
                detail: { category: category }
            });
            document.dispatchEvent(event);
            
            console.log(`Selected category: ${category}`);
        });
    });
    
    window.addEventListener('resize', () => {
        if (items.length > 0) {
            itemWidth = items[0].offsetWidth + gap;
            
            const maxScroll = getMaxScroll();
            if (currentPosition > maxScroll) {
                currentPosition = maxScroll;
                updateCarouselPosition();
            }
        }
    });
});
