FROM node:22-alpine
WORKDIR /app

COPY blogApp/BlogAPP_FrontVue/package*.json ./
RUN npm ci

COPY blogApp/BlogAPP_FrontVue/ ./

EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]
