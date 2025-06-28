document.addEventListener("DOMContentLoaded", function () {
    const entries = document.querySelectorAll(".typed-response");
    entries.forEach(el => {
        const audio = new Audio("https://cdn.pixabay.com/download/audio/2023/03/02/audio_2cf77cf0b2.mp3?filename=typing-80869.mp3");
        const text = el.getAttribute("data-answer");
        let i = 0;
        let paused = false;
        const controller = document.createElement("span");
        controller.className = "cursor-control btn btn-sm btn-light ms-2";
        controller.textContent = "⏸";
        el.parentNode.appendChild(controller);

        controller.onclick = () => {
            paused = !paused;
            controller.textContent = paused ? "▶️" : "⏸";
        };

        function type() {
            if (paused) {
                setTimeout(type, 100);
            } else if (i < text.length) {
                el.textContent += text.charAt(i);
                i++;
                audio.currentTime = 0;
                audio.play();
                setTimeout(type, 8);
            }
        }
        type();


    });
});

