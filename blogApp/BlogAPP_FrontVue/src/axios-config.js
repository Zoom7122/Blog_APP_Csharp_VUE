import axios from 'axios';

const instance = axios.create({
    baseURL: 'https://localhost:7284/api',
    withCredentials: true, // ВАЖНО: отправляем куки с каждым запросом
    headers: {
        'Content-Type': 'application/json'
    }
});

// Перехватчик для обработки ошибок аутентификации
instance.interceptors.response.use(
    response => response,
    error => {
        if (error.response && error.response.status === 401) {
            // Пользователь не авторизован
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default instance;