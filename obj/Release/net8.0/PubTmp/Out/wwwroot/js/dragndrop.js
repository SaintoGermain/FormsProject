import Sortable from 'sortablejs';

const el = document.getElementById("question-list");
Sortable.create(el, {
    animation: 150,
    onEnd: (evt) => {
        console.log("Pregunta movida de posición:", evt.oldIndex, "a", evt.newIndex);
    },
});