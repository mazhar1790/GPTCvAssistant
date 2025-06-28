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

        addChatEntry(exchange) {
            const historyContainer = $('#chatHistory');

            // Remove "no history" message if it exists
            historyContainer.find('.text-center.text-muted').remove();

            const entryHtml = `
            <div class="chat-entry">
                <p><strong><i class="fas fa-user me-2"></i>You:</strong> ${this.escapeHtml(exchange.userQuestion)}</p>
                <p><strong><i class="fas fa-robot me-2"></i>GPT:</strong> ${this.escapeHtml(exchange.answer)}</p>
            </div>`;

            historyContainer.append(entryHtml);
        }

    });
});

