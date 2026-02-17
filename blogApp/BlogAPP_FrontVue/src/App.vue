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
import Login from './components/Login.vue';
import Registration from './components/RegisterPage.vue';
import Dashboard from './components/Dashboard.vue';
import InfAboutConnecting from './components/InfAboutConnecting.vue';



import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7284/api',
  withCredentials: true,
  headers: { 'Content-Type': 'application/json' }
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

     normalizeUser(userData) {
      if (!userData) return null;

      return {
        name: userData.name || userData.Name || '',
        email: userData.email || userData.Email || '',
        avatar_url: userData.avatar_url || userData.avatar || userData.Avatar || '',
        role: userData.role || userData.Role || 'User',
        countPost: Number(userData.countPost ?? userData.CountPost ?? 0),
        bio: userData.bio || userData.Bio || '',
        CountCommetsUser: Number(userData.CountCommetsUser ?? 0)
      };
    },

    handleLoginSuccess(userData) {
      console.log('Получены данные пользователя:', userData);
      this.isLoggedIn = true;
      const normalizedUser = this.normalizeUser(userData);
      this.userData = normalizedUser;
      localStorage.setItem('user', JSON.stringify(normalizedUser));
    },
    
    handleRegistrationSuccess(registrationData) {
      console.log('Регистрация успешна:', registrationData);
      this.currentPage = 'login';
      alert('Регистрация успешна! Теперь вы можете войти в систему.');
    },

    
    handleProfileUpdated(updatedUser) {
      const normalizedUser = this.normalizeUser(updatedUser);
      this.userData = normalizedUser;
      localStorage.setItem('user', JSON.stringify(normalizedUser));
    },
    
    // Метод проверки авторизации
    async checkAuth() {
      try {
        const response = await api.get('/Entrance/CheckAuth');

        console.log('Ответ checkAuth:', response.data);
        
        if (response.data.success) {
          this.isLoggedIn = true;
         const normalizedUser = this.normalizeUser(response.data.user || response.data);
          this.userData = normalizedUser;
          localStorage.setItem('user', JSON.stringify(normalizedUser));
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
        await api.post('/Entrance/Logout');
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