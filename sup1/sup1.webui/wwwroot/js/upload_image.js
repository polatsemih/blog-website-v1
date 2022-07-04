document.querySelector('#upload-image-input').onchange = function() {
    var reader = new FileReader();

    reader.readAsDataURL(this.files[0]);

    reader.onload = function(e) {
        document.querySelector('#upload-image').src = e.target.result;
    };
};