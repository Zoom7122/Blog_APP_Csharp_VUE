<template>

<div class="app">
    <h1>Login</h1>
    <p>Введите данные и нажмите кнопку для отправки на сервер</p>
    
    <div class="form">
      <div class="form-group">
        <label>Email адрес:</label>
        <input 
          type="text" 
          v-model="email"
          placeholder="test@example.com"
          class="input-field"
        />
      </div>
      
      <div class="form-group">
        <label>Пароль:</label>
        <input 
          type="password" 
          v-model="password"
          placeholder="123456"
          class="input-field"
        />
      </div>
      
      <button 
        @click="sendData" 
        :disabled="loading"
        class="submit-btn"
      >
        {{ loading ? 'Отправка...' : 'Отправить в C# API' }}
      </button>
    </div>

    <div v-show="ApiResponce == 'false'" class="ApiResponceFalse">
        <p>Неверный Email или Пароль</p>
    </div>
    
    <!-- Результат от сервера -->
    <div v-if="ApiResponce" class="ApiResponce-box">
      <h3>Ответ от C# сервера:</h3>
      <pre class="ApiResponce-content">{{ ApiResponce }}</pre>
    </div>
    
    <!-- Ошибка -->
    <div v-if="error" class="error-box">
      <strong>Ошибка:</strong> {{ error }}
    </div>

    <div v-if="error" class="error-box">
      <strong>Ошибка:</strong> {{ error }}
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
            const response = await api.post('/EntranceConroller/Login', dataToSend);
            
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