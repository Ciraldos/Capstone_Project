
window.addEventListener('load', function () {
    document.getElementById('start').classList.add('fade-in');
});

document.addEventListener('DOMContentLoaded', function () {
    // Dichiarazione delle variabili
    const bottomLeftTexts = ['Club', 'Dance', 'Repeat'];
    const topRightTexts = ['Move', 'Groove', 'Unite'];
    const bottomRightTexts = ['Beats', 'Lights', 'Nights'];
    const topLeftTexts = ['Rave', 'Vibe', 'Elevate'];
    const centerText = 'TON?GHT';

    let centerCharIndex = 0;
    let centerDeleting = false;

    let currentTopRightIndex = 0;
    let currentBottomRightIndex = 0;
    let currentTopLeftIndex = 0;
    let currentBottomLeftIndex = 0;



    let bottomLeftCharIndex = 0;
    let topRightCharIndex = 0;
    let bottomRightCharIndex = 0;
    let topLeftCharIndex = 0;

    let bottomLeftDeleting = false;
    let topRightDeleting = false;
    let bottomRightDeleting = false;
    let topLeftDeleting = false;

    const typingSpeed = 300;
    const deletingSpeed = 100;
    const typingSpeedCenter = 500;
    const deletingSpeedCenter = 300;

    function typeBottomLeft() {
        const textElement = document.getElementById('typing-text-bottom-left');
        if (!textElement) return; // Controllo per evitare errori se l'elemento non esiste
        const currentText = bottomLeftTexts[currentBottomLeftIndex];

        if (bottomLeftDeleting) {
            if (bottomLeftCharIndex > 0) {
                textElement.textContent = currentText.substring(0, bottomLeftCharIndex - 1) + '|';
                bottomLeftCharIndex--;
                setTimeout(typeBottomLeft, deletingSpeed);
            } else {
                bottomLeftDeleting = false;
                currentBottomLeftIndex = (currentBottomLeftIndex + 1) % bottomLeftTexts.length;
                setTimeout(typeBottomLeft, typingSpeed);
            }
        } else {
            if (bottomLeftCharIndex < currentText.length) {
                textElement.textContent = currentText.substring(0, bottomLeftCharIndex + 1) + '|';
                bottomLeftCharIndex++;
                setTimeout(typeBottomLeft, typingSpeed);
            } else {
                bottomLeftDeleting = true;
                setTimeout(typeBottomLeft, typingSpeed);
            }
        }
    }

    function typeTopRight() {
        const textElement = document.getElementById('typing-text-top-right');
        if (!textElement) return; // Controllo per evitare errori se l'elemento non esiste
        const currentText = topRightTexts[currentTopRightIndex];

        if (topRightDeleting) {
            if (topRightCharIndex > 0) {
                textElement.textContent = currentText.substring(0, topRightCharIndex - 1) + '|';
                topRightCharIndex--;
                setTimeout(typeTopRight, deletingSpeed);
            } else {
                topRightDeleting = false;
                currentTopRightIndex = (currentTopRightIndex + 1) % topRightTexts.length;
                setTimeout(typeTopRight, typingSpeed);
            }
        } else {
            if (topRightCharIndex < currentText.length) {
                textElement.textContent = currentText.substring(0, topRightCharIndex + 1) + '|';
                topRightCharIndex++;
                setTimeout(typeTopRight, typingSpeed);
            } else {
                topRightDeleting = true;
                setTimeout(typeTopRight, typingSpeed);
            }
        }
    }


    function typeBottomRight() {
        const textElement = document.getElementById('typing-text-bottom-right');
        if (!textElement) return; // Controllo per evitare errori se l'elemento non esiste
        const currentText = bottomRightTexts[currentBottomRightIndex];

        if (bottomRightDeleting) {
            if (bottomRightCharIndex > 0) {
                textElement.textContent = currentText.substring(0, bottomRightCharIndex - 1) + '|';
                bottomRightCharIndex--;
                setTimeout(typeBottomRight, deletingSpeed);
            } else {
                bottomRightDeleting = false;
                currentBottomRightIndex = (currentBottomRightIndex + 1) % bottomRightTexts.length;
                setTimeout(typeBottomRight, typingSpeed);
            }
        } else {
            if (bottomRightCharIndex < currentText.length) {
                textElement.textContent = currentText.substring(0, bottomRightCharIndex + 1) + '|';
                bottomRightCharIndex++;
                setTimeout(typeBottomRight, typingSpeed);
            } else {
                bottomRightDeleting = true;
                setTimeout(typeBottomRight, typingSpeed);
            }
        }
    }


    function typeTopLeft() {
        const textElement = document.getElementById('typing-text-top-left');
        if (!textElement) return; // Controllo per evitare errori se l'elemento non esiste
        const currentText = topLeftTexts[currentTopLeftIndex];

        if (topLeftDeleting) {
            if (topLeftCharIndex > 0) {
                textElement.textContent = currentText.substring(0, topLeftCharIndex - 1) + '|';
                topLeftCharIndex--;
                setTimeout(typeTopLeft, deletingSpeed);
            } else {
                topLeftDeleting = false;
                currentTopLeftIndex = (currentTopLeftIndex + 1) % topLeftTexts.length;
                setTimeout(typeTopLeft, typingSpeed);
            }
        } else {
            if (topLeftCharIndex < currentText.length) {
                textElement.textContent = currentText.substring(0, topLeftCharIndex + 1) + '|';
                topLeftCharIndex++;
                setTimeout(typeTopLeft, typingSpeed);
            } else {
                topLeftDeleting = true;
                setTimeout(typeTopLeft, typingSpeed);
            }
        }
    }

    function typeCenter() {
        const textElement = document.getElementById('typing-text-center');
        if (!textElement) return; // Controllo per evitare errori se l'elemento non esiste

        if (centerDeleting) {
            if (centerCharIndex > 0) {
                textElement.textContent = centerText.substring(0, centerCharIndex - 1) + '|';
                centerCharIndex--;
                setTimeout(typeCenter, deletingSpeedCenter);
            } else {
                centerDeleting = false;
                setTimeout(typeCenter, typingSpeedCenter);
            }
        } else {
            if (centerCharIndex < centerText.length) {
                textElement.textContent = centerText.substring(0, centerCharIndex + 1) + '|';
                centerCharIndex++;
                setTimeout(typeCenter, typingSpeedCenter);
            } else {
                centerDeleting = true;
                setTimeout(typeCenter, typingSpeedCenter);
            }
        }
    }

    // Avvia l'animazione del testo
    typeCenter();
    typeBottomLeft();
    typeTopRight();
    typeBottomRight();
    typeTopLeft();
});


