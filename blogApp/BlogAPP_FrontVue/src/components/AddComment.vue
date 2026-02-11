<template>
  <div class="add-comment">
    <h3 class="comment-title">Добавить комментарий</h3>
    
    <!-- Форма комментария -->
    <div v-if="!CommentIsSend" class="comment-form">
      <div class="form-group">
        <textarea 
          v-model="ContentCommetn" 
          placeholder="Напишите ваш комментарий..."
          rows="4"
          class="comment-textarea"
        ></textarea>
        <div class="char-count" v-if="ContentCommetn.length > 0">
          {{ ContentCommetn.length }} символов
        </div>
      </div>
      
      <button 
        @click="SendComment()" 
        class="submit-button"
        :disabled="!ContentCommetn.trim()"
      >
        Отправить комментарий
      </button>
      
      <!-- Индикатор загрузки -->
      <div v-if="loading" class="loading-indicator">
        Отправка...
      </div>
    </div>
    
    <!-- Сообщение об успешной отправке -->
    <div v-if="CommentIsSend" class="success-message">
      <div class="success-icon">✓</div>
      <div class="success-text">
        <h4>Комментарий успешно отправлен!</h4>
        <p>Ваш комментарий будет отображен после проверки.</p>
      </div>
      <button @click="resetForm()" class="add-another-button">
        Добавить ещё комментарий
      </button>
    </div>
    
    <!-- Сообщение об ошибке -->
    <div v-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </div>
  </div>
</template>

<script>
import api from '@/axios-config';


export default {
  name: 'AddComment',
  
  props: {
    articleId: {
      type: String,
      default: null
    }
  },
  
  data() {
    return {
      ContentCommetn: "",
      CommentIsSend: false,
      loading: false,
      errorMessage: ""
    }
  },
  
  methods: {
    async SendComment() {
      if (!this.ContentCommetn.trim()) {
        this.errorMessage = "Пожалуйста, напишите комментарий";
        return;
      }
      
      if (!this.articleId) {
        this.errorMessage = "Не удалось определить статью";
        return;
      }
      
      this.loading = true;
      this.errorMessage = "";
      
      try {
        console.log('Отправка комментария для статьи: ' + this.articleId);
        
        const Comment = {
          Content: this.ContentCommetn,
          ArticleId: this.articleId,
          UserId: null,
          CreatedAt: null
        };
        
        const response = await api.post("/Commets/CreateComments", Comment);
        
        console.log('Ответ от API: ', response.data);
        console.log('Успех: ' + response.data.success);
        
        if (response.data.success) {
          this.CommentIsSend = true;
          // Опционально: emit события для родительского компонента
          this.$emit('comment-sent', {
            articleId: this.articleId,
            content: this.ContentCommetn
          });
        } else {
          this.errorMessage = response.data.message || "Ошибка при отправке комментария";
        }
      } catch (error) {
        console.error('Ошибка API:', error);
        this.errorMessage = "Произошла ошибка при отправке. Попробуйте снова.";
      } finally {
        this.loading = false;
      }
    },
    
    resetForm() {
      this.ContentCommetn = "";
      this.CommentIsSend = false;
      this.errorMessage = "";
    }
  }
}
</script>

<style scoped>
@import '@/components/css/AddComment.css';
</style>