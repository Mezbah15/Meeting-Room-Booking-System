/*Booking Delete Confirm Close Button: Booking/Index*/

$('#modal-close-btn').click(function () {
    $('#DeleteConfirmModal').none;
    $('.overlay').hide();
});
$('.overlay').on('click', function (e) {
    if (e.target !== this) {
        return;
    }
    $('.overlay').hide();
});
