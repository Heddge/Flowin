
window.flowinNoteListAnimation = (() => {
    let previousPositions = new Map();

    function capture() {
        previousPositions.clear();

        document.querySelectorAll(".note-list-item-shell").forEach(item => {
            previousPositions.set(
                item.dataset.noteId,
                item.getBoundingClientRect().top
            );
        });
    }

    function animate(newNoteId) {
        const items = document.querySelectorAll(".note-list-item-shell");

        items.forEach(item => {
            const id = item.dataset.noteId;

            // Анимация появления новой заметки
            if (id === newNoteId) {
                item.animate(
                    [
                        {
                            transform: "translateX(-30px)",
                            opacity: 0
                        },
                        {
                            transform: "translateX(0)",
                            opacity: 1
                        }
                    ],
                    {
                        duration: 320,
                        easing: "cubic-bezier(0.22, 1, 0.36, 1)"
                    }
                );

                return;
            }

            // Анимация перемещения существующих заметок
            const oldTop = previousPositions.get(id);

            if (oldTop === undefined)
                return;

            const newTop = item.getBoundingClientRect().top;
            const difference = oldTop - newTop;

            if (Math.abs(difference) < 1)
                return;

            item.animate(
                [
                    {
                        transform: `translateY(${difference}px)`
                    },
                    {
                        transform: "translateY(0)"
                    }
                ],
                {
                    duration: 320,
                    easing: "cubic-bezier(0.22, 1, 0.36, 1)"
                }
            );
        });

        previousPositions.clear();
    }

    return { capture, animate };
})();
