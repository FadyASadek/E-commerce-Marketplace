document.addEventListener('DOMContentLoaded', function () {

    const mainImageInput = document.getElementById('mainImage');
    const mainImagePreview = document.getElementById('mainImagePreview');

    const subImagesInput = document.getElementById('additionalImages'); 

    const subImagesPreview = document.getElementById('subImagesPreview');
    const productForm = document.getElementById('productForm');

    function displayImagePreview(input, previewElement, multiple) {
        previewElement.innerHTML = '';

        if (input.files && input.files.length > 0) {
            if (multiple) {
                Array.from(input.files).forEach(file => {
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const container = document.createElement('div');
                        container.className = 'c7'; 

                        const img = document.createElement('img');
                        img.src = e.target.result;

                        container.appendChild(img);
                        previewElement.appendChild(container);
                    };
                    reader.readAsDataURL(file);
                });
            } else {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    previewElement.appendChild(img);
                };
                reader.readAsDataURL(input.files[0]);
            }
        } else {
            previewElement.innerHTML = '<p>No images selected</p>';
        }
    }

    

    
    mainImageInput.addEventListener('change', function () {
        displayImagePreview(this, mainImagePreview, false);
    });

    subImagesInput.addEventListener('change', function () {
        displayImagePreview(this, subImagesPreview, true);
    });

});