document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const loginBtnText = document.getElementById('loginBtnText');
    const loginSpinner = document.getElementById('loginSpinner');
    const loginAlert = document.getElementById('loginAlert');
    const togglePassword = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('password');

    // Check if already logged in
    if (AuthService.isLoggedIn() && window.location.pathname.includes('index.html')) {
        window.location.href = 'dashboard.html';
    }

    // Toggle Password Visibility
    if (togglePassword) {
        togglePassword.addEventListener('click', () => {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);
            togglePassword.querySelector('i').classList.toggle('fa-eye');
            togglePassword.querySelector('i').classList.toggle('fa-eye-slash');
        });
    }

    // Handle Login Form Submission
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const username = document.getElementById('username').value;
            const password = passwordInput.value;

            // Show Loading State
            loginBtnText.classList.add('d-none');
            loginSpinner.classList.remove('d-none');
            loginAlert.classList.add('d-none');
            loginForm.querySelector('button[type="submit"]').disabled = true;

            try {
                await AuthService.login(username, password);

                // Show Success
                loginBtnText.innerText = 'Success!';
                loginBtnText.classList.remove('d-none');
                loginSpinner.classList.add('d-none');

                // Redirect
                setTimeout(() => {
                    window.location.href = 'dashboard.html';
                }, 800);

            } catch (error) {
                // Show Error
                console.error(error);
                loginAlert.innerText = error.message || 'Invalid username or password.';
                loginAlert.classList.remove('d-none');

                // Reset Loading State
                loginBtnText.innerText = 'Log In';
                loginBtnText.classList.remove('d-none');
                loginSpinner.classList.add('d-none');
                loginForm.querySelector('button[type="submit"]').disabled = false;

                // Add shake animation
                loginAlert.classList.remove('animate__shakeX');
                void loginAlert.offsetWidth; // trigger reflow
                loginAlert.classList.add('animate__shakeX');
            }
        });
    }
});
