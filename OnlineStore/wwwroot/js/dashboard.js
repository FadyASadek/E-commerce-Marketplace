document.addEventListener('DOMContentLoaded', () => {
    const toggleButton = document.querySelector('.toggle-sidebar');
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');
    
    function updateLayout() {
        if (sidebar.classList.contains('hidden')) {
            mainContent.style.marginLeft = '0';
            mainContent.style.paddingLeft = '0';
        } else {
            mainContent.style.marginLeft = '250px';
            mainContent.style.paddingLeft = '0';
        }
    }
    
    updateLayout();
    
    if (toggleButton) {
        toggleButton.addEventListener('click', () => {
            sidebar.classList.toggle('hidden');
            updateLayout();
        });
    }

    
    const addProductBtn = document.querySelector('.add-product-btn');
    const productForm = document.querySelector('.product-form');
    const cancelBtn = document.querySelector('.cancel-btn');

    addProductBtn.addEventListener('click', () => {
        productForm.style.display = 'block';
    });

    cancelBtn.addEventListener('click', () => {
        productForm.style.display = 'none';
        productForm.reset();
    });

    window.addEventListener('click', (e) => {
        if (e.target === productForm) {
            productForm.style.display = 'none';
            productForm.reset();
        }
    });

    const productFormElement = document.getElementById('productForm');
    productFormElement.addEventListener('submit', (e) => {
        e.preventDefault();
        
        const formData = new FormData(productFormElement);
        const productData = {
            name: formData.get('productName'),
            price: formData.get('productPrice'),
            description: formData.get('productDescription'),
            image: formData.get('productImage')
        };

        console.log('Product submitted:', productData);
        
        productForm.style.display = 'none';
        productForm.reset();
    });
});
