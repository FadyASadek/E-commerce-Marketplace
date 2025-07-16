$(document).ready(function() {
    const updateCart = (itemId, newQuantity) => {
        const itemRow = $(`.cart-item[data-item-id="${itemId}"]`);
        itemRow.css('opacity', 0.5);

        $.ajax({
            url: '/Cart/UpdateQuantity', 
            type: 'POST',
            data: { itemId: itemId, newQuantity: newQuantity },
            success: function(response) {
                if (response.success) {
                    location.reload(); 
                } else {
                    Swal.fire('Update Failed', response.message || 'Could not update the cart.', 'error');
                    itemRow.css('opacity', 1);
                }
            },
            error: function(xhr) {
                const errorResponse = xhr.responseJSON;
                Swal.fire('Error!', errorResponse ? errorResponse.message : 'An unknown error occurred.', 'error');
                itemRow.css('opacity', 1);
            }
        });
    };

    $('.increase-qty-btn').on('click', function() {
        const itemId = $(this).data('item-id');
        const input = $(this).siblings('.quantity-input');
        const currentQty = parseInt(input.val());
        updateCart(itemId, currentQty + 1);
    });

    $('.decrease-qty-btn').on('click', function() {
        const itemId = $(this).data('item-id');
        const input = $(this).siblings('.quantity-input');
        const currentQty = parseInt(input.val());
        if (currentQty > 1) {
            updateCart(itemId, currentQty - 1);
        }
    });

    $('.remove-btn').on('click', function(e) {
        e.preventDefault();
        const itemId = $(this).data('item-id');
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, remove it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Cart/RemoveFromCart', 
                    type: 'POST',
                    data: { itemId: itemId },
                    success: function(response) {
                        if (response.success) {
                            location.reload();
                        } else {
                             Swal.fire('Failed', response.message || 'Could not remove item.', 'error');
                        }
                    },
                    error: function() {
                        Swal.fire('Error!', 'Could not remove item.', 'error');
                    }
                });
            }
        });
    });
});
