pipeline {

  agent any

  environment {
    EMAIL_TO = 'taleleyashwant@gmail.com'
  }

  stages {
    stage ('checkout'){
      steps{
        checkout scm
      }
    }

    stage('Docker Build') { 
      agent none
      steps {
        script {
          sh 'docker build -t gauravtalele/angular-jenkins-cicd:latest .'
        }
      }
    }

    stage('Docker push') { 
      agent none
      steps {
        script {
          sh "docker login -u 'gauravtalele' -p 'gauravtalele*123'"
          sh 'docker push gauravtalele/angular-jenkins-cicd:latest'
        }
      }
    }
  }
}
    