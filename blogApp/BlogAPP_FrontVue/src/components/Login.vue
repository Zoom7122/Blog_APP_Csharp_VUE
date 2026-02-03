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
      email: 'ivan@example.com',
      password: '2313213',
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
                // ✅ Успешный вход
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

<style>

/* Базовые стили */
body {
  font-family: 'Arial', sans-serif;
  margin: 0;
  padding: 20px;
  background-color: #f5f5f5;
}

.app {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  background: white;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

h1 {
  color: #333;
  margin-bottom: 10px;
}

p {
  color: #666;
  margin-bottom: 20px;
}

/* Стили формы */
.form {
  margin: 30px 0;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 8px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: bold;
  color: #444;
}

.input-field {
  width: 100%;
  max-width: 400px;
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
  transition: border-color 0.3s;
}

.input-field:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 2px rgba(66, 184, 131, 0.2);
}

.submit-btn {
  padding: 12px 25px;
  background: #42b883;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
}

.submit-btn:hover:not(:disabled) {
  background: #3aa876;
}

.submit-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Стили для результата */
.ApiResponce-box {
  margin: 25px 0;
  padding: 20px;
  background: #e8f5e9;
  border-radius: 8px;
  border-left: 4px solid #4caf50;
}

.ApiResponce-box h3 {
  color: #2e7d32;
  margin-top: 0;
}

.ApiResponce-content {
  background: white;
  padding: 15px;
  border-radius: 4px;
  overflow: auto;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 14px;
  line-height: 1.4;
}

/* Стили для ошибок */
.error-box {
  margin: 25px 0;
  padding: 15px;
  background: #ffebee;
  border-radius: 8px;
  border-left: 4px solid #f44336;
  color: #c62828;
}

.ApiResponceFalse p{
    font-size: 20;
    color: red;
}

</style>