document.addEventListener("DOMContentLoaded", function () {

    const toggleBtn = document.getElementById("menuToggle");
    const navList = document.getElementById("navList");

    toggleBtn.addEventListener("click", function () {

        navList.classList.toggle("active");

        //Byt ikon
        if (navList.classList.contains("active")) {
            toggleBtn.textContent = "✖";
        } else {
            toggleBtn.textContent = "☰";
        }

    });

});
