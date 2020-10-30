FROM node:10-alpine AS compile-image
RUN npm install -g @angular/cli
WORKDIR /app
COPY package.json .
RUN npm install
ENV PATH="./node_modules/.bin:$PATH" 
COPY . .
RUN ng build --prod

FROM nginx:1.19.2
COPY default.conf.template /etc/nginx/conf.d/default.conf.template
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=compile-image /app/dist/client /usr/share/nginx/html
CMD /bin/bash -c "envsubst '\$PORT' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf" && nginx -g 'daemon off;'
EXPOSE 80