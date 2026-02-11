<template>
  <div class="app-container">
    <!-- Текущая страница -->
    <div class="page-content">
      <Login 
        v-if="currentPage === 'login' && !isLoggedIn" 
        @success="handleLoginSuccess"
        @navigate-to-register="currentPage = 'register'"
      />
      
      <Registration 
        v-if="currentPage === 'register' && !isLoggedIn" 
        @registration-success="handleRegistrationSuccess"
        @navigate-to-login="currentPage = 'login'"
      />
      
      <!-- После успешного входа -->
      <Dashboard 
        v-if="isLoggedIn" 
        :user="userData"
        @logout="handleLogout"
      />
    </div>

    <nav class="app-nav" v-if="!isLoggedIn">
      <button 
        @click="currentPage = 'login'" 
        :class="{ active: currentPage === 'login' }"
        class="nav-btn"
      >
        Вход
      </button>
      <button 
        @click="currentPage = 'register'" 
        :class="{ active: currentPage === 'register' }"
        class="nav-btn"
      >
        Регистрация
      </button>
    </nav>


    <InfAboutConnecting v-if="!isLoggedIn" />
  </div>
</template>

<script>
import axios from 'axios'; 
import Login from './components/Login.vue';
import Registration from './components/RegisterPage.vue';
import Dashboard from './components/Dashboard.vue';
import InfAboutConnecting from './components/InfAboutConnecting.vue';


const api = axios.create({
  baseURL: 'https://localhost:7284/api',
  withCredentials: true, 
  headers: {
    'Content-Type': 'application/json'
  }
});

export default {
  name: 'App',
  components: {
    Login,
    Registration,
    Dashboard,
    InfAboutConnecting
  },
  data() {
    return {
      currentPage: 'login',
      isLoggedIn: false,
      userData: null
    };
  },
  methods: {
    // ✅ ДОБАВЛЕНО: Метод для обработки успешного входа
    handleLoginSuccess(userData) {
      console.log('Получены данные пользователя:', userData);
      this.isLoggedIn = true;
      this.userData = userData;
      localStorage.setItem('user', JSON.stringify(userData));
    },
    
    // ✅ ДОБАВЛЕНО: Метод для обработки успешной регистрации
    handleRegistrationSuccess(registrationData) {
      console.log('Регистрация успешна:', registrationData);
      this.currentPage = 'login';
      alert('Регистрация успешна! Теперь вы можете войти в систему.');
    },
    
    // Метод проверки авторизации
    async checkAuth() {
      try {
        const response = await api.get('/EntranceConroller/CheckAuth');

        console.log('Ответ checkAuth:', response.data);
        
        if (response.data.success) {
          this.isLoggedIn = true;
          this.userData = response.data.user || response.data;
          localStorage.setItem('user', JSON.stringify(response.data.user));
        }
      } catch (error) {
        console.log('Пользователь не авторизован');
        this.isLoggedIn = false;
        this.userData = null;
        localStorage.removeItem('user');
      }
    },
    
    // Метод выхода
    async handleLogout() {
      try {
        await api.post('/EntranceConroller/Logout');
      } catch (error) {
        console.error('Ошибка при выходе:', error);
      }
      this.isLoggedIn = false;
      this.userData = null;
      localStorage.removeItem('user');
      this.currentPage = 'login';
      console.log('Выход выполнен');
    }
  },
  mounted() {
    this.checkAuth(); // Проверяем аутентификацию при загрузке
  }
};
</script>

<style scoped>
@import '@/components/css/appVue.css';
</style>