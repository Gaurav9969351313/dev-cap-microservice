https://github.com/petenorth/angular-5-example/blob/master/jenkins/Jenkinsfile

https://github.com/hms-dbmi/avillachlab-jenkins/blob/master/jenkins-docker/Dockerfile
https://medium.com/@karthi.net/docker-tutorial-build-docker-images-using-jenkins-d2880e65b74
https://tutorials.releaseworksacademy.com/learn/building-your-first-docker-image-with-jenkins-2-guide-for-developers

https://www.liatrio.com/blog/building-with-docker-using-jenkins-pipelines

Step 1: docker-compose up -d
Step 2: docker exec -it jenkins bash
Step 3: docker exec /var/jenkins_home/secrets/initialAdminPassword
Step 4: copy the token and unlock the jenkins and setup the username and password
Step 5: Home ==> manage jenkins ==> available ==>

install following plugins

Docker Pipeline
Docker
Email Extension Template

Step 6: Restart the jenkins or reload the jenkins
Step 7: Create the pipeline ==> Multibrach pipeline ==> Branch Sources ==> Select Github ==>
Click on Add (Dropdown will open) ==> Select Jenkins ==> Add Git Credentials
then add git repo url
Step 8: save
Step 9: Setting Up Email Notifications
Manage Jenkins ==> Configure System ==> E-mail Notification & Extended E-mail Notification
configure ith same credentials

SMTP Server: smtp.gmail.com
Use SMTP Authentication: Check this box (true)
User Name: gauravtalele@gmail.com
password: <Email Password>
Use SSL: true
SMTP Port: 465
Charset: utf-8

gau

agent {
docker {
image 'node:10-alpine'
args '-p 3000:3000'
}
}

    stage('Clean Workspace'){
        steps {
            cleanWs()
        }
    }

    stage('Restore') {
        steps {
            sh 'npm install'
        }
    }

    stage('Build') {
        steps {
          sh 'npm run build'
        }
    }

docker image build -t gauravtalele/angular-jenkins-cicd .
docker container run -d -p 8085:80 gauravtalele/angular-jenkins-cicd

stage('Build and push') {
steps {
script {
def apitestimage = docker.build("gauravtalele/angular-jenkins-cicd:\${env.BUILD_ID}")  
 }
}
}
=========================
# this is correct
Heroku

wotoveh508@treeheir.com
gauravqwerty@123

heroku login
heroku container:login

docker build -t gauravtalele/angular-jenkins-cicd .
docker tag gauravtalele/angular-jenkins-cicd registry.heroku.com/angular-heroku-app-1102/client
docker push registry.heroku.com/angular-heroku-app-1102/client
heroku container:release client --app=angular-heroku-app-1102
heroku ps:scale client=1 --app=angular-heroku-app-1102
heroku logs --tail -a angular-heroku-app-1102


heroku apps 
heroku open
heroku ps -a angular-heroku-app-1102


=============================================================================
https://itnext.io/how-to-deploy-angular-application-to-heroku-1d56e09c5147


