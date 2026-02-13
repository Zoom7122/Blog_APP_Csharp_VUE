<template>
  <div class="registration-container">
    <h1>Регистрация</h1>
    <p>Создайте новый аккаунт для доступа к системе</p>
    
    <!-- Форма регистрации -->
    <div class="form">
      <!-- Имя пользователя -->
      <div class="form-group">
        <label>Имя пользователя:</label>
        <input 
          type="text" 
          v-model="firstName"
          placeholder="Иван"
          class="input-field"
          :class="{ 'error-input': errors.firstName }"
        />
        <div v-if="errors.firstName" class="error-message">
          {{ errors.firstName }}
        </div>
      </div>
      
      <!-- Email -->
      <div class="form-group">
        <label>Email адрес:</label>
        <input 
          type="email" 
          v-model="email"
          placeholder="ivan@example.com"
          class="input-field"
          :class="{ 'error-input': errors.email }"
        />
        <div v-if="errors.email" class="error-message">
          {{ errors.email }}
        </div>
      </div>
      
      <!-- Пароль -->
      <div class="form-group">
        <label>Пароль:</label>
        <input 
          type="password" 
          v-model="password"
          placeholder="Минимум 8 символов"
          class="input-field"
          :class="{ 'error-input': errors.password }"
        />
        <div v-if="errors.password" class="error-message">
          {{ errors.password }}
        </div>
      </div>
      
      <!-- Подтверждение пароля -->
      <div class="form-group">
        <label>Подтверждение пароля:</label>
        <input 
          type="password" 
          v-model="confirmPassword"
          placeholder="Повторите пароль"
          class="input-field"
          :class="{ 'error-input': errors.confirmPassword }"
        />
        <div v-if="errors.confirmPassword" class="error-message">
          {{ errors.confirmPassword }}
        </div>
      </div>
      
      <!-- Аватар (опционально) -->
      <div class="form-group">
        <label>Ссылка на аватар (необязательно):</label>
        <input 
          type="text" 
          v-model="avatar_url"
          placeholder="https://example.com/avatar.jpg"
          class="input-field"
        />
      </div>
      
      <!-- Биография (опционально) -->
      <div class="form-group">
        <label>О себе (необязательно):</label>
        <textarea 
          v-model="bio"
          placeholder="Расскажите немного о себе..."
          class="input-field textarea"
          rows="3"
        ></textarea>
      </div>
      
      <!-- Роль (обычно скрыто или select) -->
      <div class="form-group" v-if="showRoleSelect">
        <label>Роль:</label>
        <select v-model="role" class="input-field">
          <option value="User">Пользователь</option>
          <option value="Admin">Администратор</option>
          <option value="Moderator">Модератор</option>
        </select>
      </div>
      
      <!-- Кнопки -->
      <div class="form-actions">
        <button 
          @click="register" 
          :disabled="loading || !isFormValid"
          class="submit-btn"
        >
          {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
        </button>
        
        <button 
          @click="goToLogin" 
          class="secondary-btn"
        >
          Уже есть аккаунт? Войти
        </button>
      </div>
    </div>
    
    <!-- Сообщение об успехе -->
    <div v-if="successMessage" class="success-box">
      <h3>✅ Успешная регистрация!</h3>
      <p>{{ successMessage }}</p>
      <button @click="goToLoginAfterSuccess" class="success-btn">
        Перейти к входу
      </button>
    </div>
    
    <!-- Ошибка регистрации -->
    <div v-if="registrationError" class="error-box">
      <h3>Ошибка регистрации:</h3>
      <p>{{ registrationError }}</p>
      <div v-if="validationErrors.length > 0" class="validation-errors">
        <ul>
          <li v-for="(error, index) in validationErrors" :key="index">
            {{ error }}
          </li>
        </ul>
      </div>
    </div>
    
    <div v-if="debug && responseData" class="debug-box">
      <h4>Ответ сервера:</h4>
      <pre>{{ responseData }}</pre>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import '@/components/css/RegisterPage.css';

export default {
  name: 'Registration',
  props: {
    showRoleSelect: {
      type: Boolean,
      default: false
    },
    debug: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      firstName: '',
      email: '',
      password: '',
      confirmPassword: '',
      avatar_url: '',
      bio: '',
      role: 'User',
      
      loading: false,
      successMessage: '',
      registrationError: '',
      responseData: null,
      validationErrors: [],
      
      errors: {
        firstName: '',
        email: '',
        password: '',
        confirmPassword: ''
      }
    }
  },
  computed: {
    isFormValid() {
      return (
        this.firstName.trim().length >= 2 &&
        this.email.trim() &&
        this.validateEmail(this.email) &&
        this.password.length >= 8 &&
        this.password === this.confirmPassword
      )
    }
  },
  watch: {
    firstName(newVal) {
      if (newVal.length > 0 && newVal.length < 2) {
        this.errors.firstName = 'Имя должно содержать минимум 2 символа';
      } else {
        this.errors.firstName = '';
      }
    },
    email(newVal) {
      if (newVal && !this.validateEmail(newVal)) {
        this.errors.email = 'Введите корректный email';
      } else {
        this.errors.email = '';
      }
    },
    password(newVal) {
      if (newVal && newVal.length < 8) {
        this.errors.password = 'Пароль должен содержать минимум 8 символов';
      } else {
        this.errors.password = '';
      }
    },
    confirmPassword(newVal) {
      if (newVal && this.password && newVal !== this.password) {
        this.errors.confirmPassword = 'Пароли не совпадают';
      } else {
        this.errors.confirmPassword = '';
      }
    }
  },
  methods: {
    validateEmail(email) {
      const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return re.test(email);
    },
    
    validateForm() {
      this.validationErrors = [];
      
      if (this.firstName.trim().length < 2) {
        this.validationErrors.push('Имя должно содержать минимум 2 символа');
      }
      
      if (!this.email.trim()) {
        this.validationErrors.push('Email обязателен');
      } else if (!this.validateEmail(this.email)) {
        this.validationErrors.push('Введите корректный email');
      }
      
      if (this.password.length < 8) {
        this.validationErrors.push('Пароль должен содержать минимум 8 символов');
      }
      
      if (this.password !== this.confirmPassword) {
        this.validationErrors.push('Пароли не совпадают');
      }
      
      return this.validationErrors.length === 0;
    },
    
    async register() {
      // Сброс предыдущих сообщений
      this.successMessage = '';
      this.registrationError = '';
      this.responseData = null;
      this.validationErrors = [];
      
      // Валидация формы
      if (!this.validateForm()) {
        this.registrationError = 'Пожалуйста, исправьте ошибки в форме';
        return;
      }
      
      this.loading = true;
      
      // Данные для отправки
      const registrationData = {
        firstName: this.firstName.trim(),
        email: this.email.trim(),
        password: this.password,
        avatar_url: this.avatar_url.trim() || null,
        bio: this.bio.trim() || null,
        role: this.role
      };
      
      console.log('Отправляю данные регистрации:', registrationData);
      
      try {
        const response = await axios.post(
          'https://localhost:7284/api/Entrance/Register', 
          registrationData,
          {
            headers: {
              'Content-Type': 'application/json'
            }
          }
        );



        
        console.log('Ответ от сервера:', response.data);
        this.responseData = response.data;
        
        if (response.data.success) {

          this.successMessage = response.data.message || 'Регистрация прошла успешно!';
          
          // Очищаем форму
          this.resetForm();
          
          // Отправляем событие успешной регистрации
          this.$emit('registration-success', response.data);
          
        } else {
          // Сервер вернул ошибку
          this.registrationError = response.data.messegeEror || 'Ошибка регистрации';
          this.validationErrors = response.data.errors || [];
        }
        
      } catch (error) {
        console.error('Ошибка при регистрации:', error);
        this.handleRegistrationError(error);
        
      } finally {
        this.loading = false;
      }
    },
    
    handleRegistrationError(error) {
      if (error.response) {
        // Сервер ответил с ошибкой
        const status = error.response.status;
        const data = error.response.data;
        
        switch (status) {
          case 400:
            this.registrationError = data.message || 'Некорректные данные';
            this.validationErrors = data.errors || [];
            break;
          case 409:
            this.registrationError = 'Пользователь с таким email уже существует';
            break;
          case 422:
            this.registrationError = 'Ошибка валидации данных';
            this.validationErrors = data.errors || [];
            break;
          default:
            this.registrationError = `Ошибка сервера: ${status}`;
        }
        
      } else if (error.request) {
        // Запрос был отправлен, но ответа нет
        this.registrationError = 'Нет ответа от сервера. Проверьте подключение.';
        
      } else {
        // Другая ошибка
        this.registrationError = `Ошибка: ${error.message}`;
      }
    },
    
    resetForm() {
      this.firstName = '';
      this.email = '';
      this.password = '';
      this.confirmPassword = '';
      this.avatar_url = '';
      this.bio = '';
      this.role = 'User';
      this.errors = {
        firstName: '',
        email: '',
        password: '',
        confirmPassword: ''
      };
    },
    
    goToLogin() {
      this.$emit('navigate-to-login');
    },
    
    goToLoginAfterSuccess() {
      this.successMessage = '';
      this.$emit('navigate-to-login');
    }
  }
}
</script>

<style scoped>

</style>