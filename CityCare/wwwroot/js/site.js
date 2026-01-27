// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// ✅ Auto-hide success alerts (global)
document.addEventListener("DOMContentLoaded", function () {
    const alerts = document.querySelectorAll(".alert-cc.success");

    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = "opacity 0.5s ease, transform 0.5s ease";
            alert.style.opacity = "0";
            alert.style.transform = "translateY(-10px)";

            setTimeout(() => {
                alert.remove();
            }, 500);
        }, 4000); // ⏱ 4git seconds
    });
});
