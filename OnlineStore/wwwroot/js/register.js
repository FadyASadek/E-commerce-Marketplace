document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    const togglePasswordButton = document.getElementById('togglePassword');
    const toggleConfirmPasswordButton = document.getElementById('toggleConfirmPassword');
    const strengthBar = document.getElementById('strengthBar');
    const strengthText = document.getElementById('strengthText');

    if (togglePasswordButton && passwordInput) {
        togglePasswordButton.addEventListener('click', function () {
            togglePasswordVisibility(passwordInput, this);
        });
    }

    if (toggleConfirmPasswordButton && confirmPasswordInput) {
        toggleConfirmPasswordButton.addEventListener('click', function () {
            togglePasswordVisibility(confirmPasswordInput, this);
        });
    }

    function togglePasswordVisibility(inputElement, button) {
        if (!inputElement) return;
        const type = inputElement.getAttribute('type') === 'password' ? 'text' : 'password';
        inputElement.setAttribute('type', type);
        const eyeIcon = button.querySelector('i');
        eyeIcon.classList.toggle('fa-eye');
        eyeIcon.classList.toggle('fa-eye-slash');
    }

    if (passwordInput) {
        passwordInput.addEventListener('input', function () {
            updatePasswordStrength(this.value);
        });
    }

    function updatePasswordStrength(password) {
        if (!strengthBar || !strengthText) return;
        const strength = calculatePasswordStrength(password);
        let strengthClass = '';
        let strengthMessage = '';
        strengthBar.style.width = strength + '%';

        if (strength === 0) {
            strengthMessage = 'Password strength';
        } else if (strength < 25) {
            strengthClass = 'very-weak';
            strengthMessage = 'Very weak';
        } else if (strength < 50) {
            strengthClass = 'weak';
            strengthMessage = 'Weak';
        } else if (strength < 75) {
            strengthClass = 'medium';
            strengthMessage = 'Medium';
        } else {
            strengthClass = 'strong';
            strengthMessage = 'Strong';
        }

        strengthBar.className = 'strength-bar';
        if (strengthClass) {
            strengthBar.classList.add(strengthClass);
        }
        strengthText.textContent = strengthMessage;
    }

    function calculatePasswordStrength(password) {
        if (!password) return 0;
        let score = 0;
        if (password.length >= 8) score += 25;
        if (/[A-Z]/.test(password)) score += 15;
        if (/[a-z]/.test(password)) score += 15;
        if (/[0-9]/.test(password)) score += 15;
        if (/[^A-Za-z0-9]/.test(password)) score += 20;
        const uniqueChars = new Set(password).size;
        score += Math.min(10, uniqueChars / password.length * 10);
        return Math.min(100, score);
    }
    
    const style = document.createElement('style');
    style.textContent = `
        .strength-bar.very-weak { background-color: #dc3545; }
        .strength-bar.weak { background-color: #ffc107; }
        .strength-bar.medium { background-color: #0d6efd; }
        .strength-bar.strong { background-color: #198754; }
    `;
    document.head.appendChild(style);
});