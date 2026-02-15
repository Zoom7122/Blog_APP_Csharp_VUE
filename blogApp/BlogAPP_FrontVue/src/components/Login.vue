<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <h1 class="login-title">Вход</h1>
        <p class="login-subtitle">Введите данные и нажмите кнопку для отправки на сервер</p>
      </div>

      <div class="form">
        <div class="form-group">
          <label class="label">Email адрес</label>
          <input
            type="text"
            v-model="email"
            placeholder="test@example.com"
            class="input-field"
          />
        </div>

        <div class="form-group">
          <label class="label">Пароль</label>
          <input
            type="password"
            v-model="password"
            placeholder="123456"
            class="input-field"
          />
        </div>

        <button
          type="button"
          @click="sendData"
          :disabled="loading"
          class="submit-btn"
        >
          <span v-if="loading" class="spinner" aria-hidden="true"></span>
          {{ loading ? 'Отправка...' : 'Войти' }}
        </button>

        <div v-if="ApiResponce == 'false'" class="notice notice-error">
          <strong>Ошибка:</strong> Неверный Email или пароль
        </div>

        <div v-if="error" class="notice notice-warning">
          <strong>Ошибка:</strong> {{ error }}
        </div>

        <div v-if="ApiResponce && ApiResponce !== 'false'" class="response">
          <div class="response-head">
            <h3 class="response-title">Ответ сервера</h3>
          </div>
          <pre class="response-content">{{ ApiResponce }}</pre>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import api from '@/axios-config';

export default {
  name: 'App',
  data() {
    return {
      email: 'testov@example.com',
      password: '12345678',
      ApiResponce: null,
      error: null,
      loading: false,
    }
  },
  methods: {
    async sendData() 
    {
      // Сброс предыдущих результатов
      this.ApiResponce = null
      this.error = null
      this.loading = true
      
      // Данные для отправки
      const dataToSend = {
        email: this.email,
        password: this.password
      }
      
      console.log('Отправляю данные на C# API:', dataToSend)
      
       try {
            const response = await api.post('/Entrance/Login', dataToSend);
            
            if (response.data && response.data.success === true) {

                this.$emit('success', response.data.user);
                
                localStorage.setItem('user', JSON.stringify(response.data.user));
                
                this.email = '';
                this.password = '';
            } else {
                this.ApiResponce = 'false';
            }
          }
       catch (err) {
        console.error('Ошибка при отправке:', err)
        
        if (err.response) {
          // Сервер ответил с ошибкой
          this.error = `Сервер вернул ошибку ${err.response.status}`
          console.log('Детали ошибки:', err.response.data)
        } else if (err.request) {
          // Запрос был отправлен, но ответа нет
          this.error = 'Нет ответа от сервера. Проверьте подключение.'
        } else {
          // Другая ошибка
          this.error = `Ошибка: ${err.message}`
        }
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

<style scoped>
@import '@/components/css/Login.css'

</style>