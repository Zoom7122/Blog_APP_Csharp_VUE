<template>
 <div class="dashboard" v-show="!showArticleForm && !showArticleView">
    <!-- Шапка Dashboard -->
    <div class="dashboard-header">
      <h1>Добро пожаловать в Dashboard!</h1>
      <button @click="logout" class="logout-btn">Выйти</button>
    </div>
    
    <!-- Информация о пользователе -->
    <div class="user-profile">
      <div class="avatar-section">
        <img 
          :src="user.avatar_url || '/default-avatar.png'" 
          :alt="user.name"
          class="avatar"
        />
      </div>
      
      <div class="user-info">
        <h2>{{ user.name }}</h2>
        <p><strong>Email:</strong> {{ user.email }}</p>
        <p><strong>Роль:</strong> {{ user.role }}</p>
        
        <div class="user-stats">
          <div class="stat">
            <span class="stat-number">{{user.countPost}}</span>
            <span class="stat-label">Статей</span>
          </div>
          <div class="stat">
            <span class="stat-number">0</span>
            <span class="stat-label">Комментариев</span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Основной контент Dashboard -->
    <div class="dashboard-content">
      <h3>Ваши возможности:</h3>
      <div class="features">
        <div class="feature-card" v-for="feature in features" :key="feature.id">
          <h4>{{ feature.title }}</h4>
          <p>{{ feature.description }}</p>
        </div>
      </div>
    </div>
    <div class="dashboard-content">

      <h3>Ваши возможности:</h3>
      <button @click="showArticleForm = true" class="add-article-btn">📝 Добавить статью</button>
      <button @click="showArticleView = true" class="add-article-btn">Найти статью</button>
    </div>
  </div>


<ArticleView v-show="showArticleView"></ArticleView>
<ArticleAdd v-show="showArticleForm" 
  @close="showArticleForm = false"
></ArticleAdd>

</template>

<script>
import ArticleAdd from './ArticleAdd.vue';
import ArticleView from './ArticleView.vue';

export default {
  components: { ArticleAdd, ArticleView},
  name: 'Dashboard',
  props: {
    // Принимаем user из App.vue
    user: {
      type: Object,
      required: true,
      default: () => ({
        name: '',
        email: '',
        avatar_url: '',
        bio: '',
        role: '',
        countPost : 0
      })
    }
  },
  data() {
    return {
      showArticleForm: false,
      showArticleView: false,
      features: [
        { id: 1, title: 'Создание статей', description: 'Пишите и публикуйте свои статьи' },
        { id: 2, title: 'Комментирование', description: 'Комментируйте статьи других пользователей' },
        { id: 3, title: 'Редактирование профиля', description: 'Настройте свой профиль' }
      ]
    };
  },
  methods: {
    async logout() {
        try {
            await api.post('/EntranceConroller/Logout');
        } catch (error) {
            console.error('Ошибка при выходе:', error);
        }
        this.$emit('logout');
    }
  },
  mounted() {
    console.log('Dashboard получил пользователя:', this.user);
  }
}
</script>

<style scoped>

.add-article-btn {
  margin: 20px 0;
  padding: 12px 24px;
  background: #42b883;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: bold;
}

.dashboard {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #42b883;
}

.logout-btn {
  padding: 10px 20px;
  background: #ff6b6b;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
}

.logout-btn:hover {
  background: #ff5252;
}

.user-profile {
  display: flex;
  gap: 30px;
  margin-bottom: 40px;
  padding: 25px;
  background: #f8f9fa;
  border-radius: 15px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.1);
}

.avatar-section {
  flex-shrink: 0;
}

.avatar {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  object-fit: cover;
  border: 5px solid white;
  box-shadow: 0 4px 10px rgba(0,0,0,0.1);
}

.user-info {
  flex-grow: 1;
}

.user-info h2 {
  margin: 0 0 10px 0;
  color: #333;
}

.user-info p {
  margin: 8px 0;
  color: #666;
}

.user-stats {
  display: flex;
  gap: 30px;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #ddd;
}

.stat {
  text-align: center;
}

.stat-number {
  display: block;
  font-size: 32px;
  font-weight: bold;
  color: #42b883;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

.dashboard-content {
  background: white;
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
}

.features {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.feature-card {
  padding: 20px;
  background: #f8f9fa;
  border-radius: 10px;
  border-left: 4px solid #42b883;
  transition: transform 0.3s;
}

.feature-card:hover {
  transform: translateY(-5px);
}
</style>