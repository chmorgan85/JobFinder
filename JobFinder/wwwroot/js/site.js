// Save/unsave forms
const formContainers = document.querySelectorAll('.myJobs-form-container');
formContainers.forEach((formContainer) => {
    const saveForm = formContainer.querySelector('.save-form');
    const unsaveForm = formContainer.querySelector('.unsave-form');
    const loader = formContainer.querySelector('.loader');

    // Save form
    saveForm.addEventListener('submit', (event) => {
        event.preventDefault();

        // Hide both forms and show loader
        saveForm.setAttribute('hidden', '');
        loader.removeAttribute('hidden');

        fetch(saveForm.getAttribute('action'), {
            method: 'post',
            body: new FormData(saveForm)
        })
            .then((response) => {
                loader.setAttribute('hidden', '');

                // If OK, show the unsave form
                if (response.ok) {
                    unsaveForm.removeAttribute('hidden');
                }

                // Otherwise, alert and show the save form again
                else {
                    alert(response.status);
                    saveForm.removeAttribute('hidden');
                }
            });
    });

    // Unsave form
    unsaveForm.addEventListener('submit', (event) => {
        event.preventDefault();

        // Hide both forms and show loader
        unsaveForm.setAttribute('hidden', '');
        loader.removeAttribute('hidden');

        fetch(unsaveForm.getAttribute('action'), {
            method: 'post',
            body: new FormData(unsaveForm)
        })
            .then((response) => {
                loader.setAttribute('hidden', '');

                // If OK, show the save form
                if (response.ok) {
                    saveForm.removeAttribute('hidden');
                }

                // Otherwise, alert and show the unsave form again
                else {
                    alert(response.status);
                    unsaveForm.removeAttribute('hidden');
                }
            });
    });
});

