let modal = null;
export function initialize(id) {
    
}

export function showModal() {
    if(modal == null) {
        modal = new bootstrap.Modal('#user-details-modal', {
            keyboard: false
        });
    }
    modal.show();
}
export function hideModal() {
    if(modal != null) {
        modal.hide();
    }
}