pipeline {
    agent any
    environment {
        DOCKER_IMAGE = 'habibelmir/dataprocessingapi:latest' // Docker image
        DOCKERHUB_CREDENTIALS = credentials('dockerhubCredential') // Docker Hub credentials
        SONAR_SCANNER_HOME = tool 'SonarScanner for MSBuild' // SonarScanner tool name configured in Jenkins
    }
    stages {
        stage('Cloner le dépôt') {
            steps {
                git branch: 'master', // Branch to clone
                    credentialsId: 'Github_Credentials', // Jenkins credentials for GitHub
                    url: 'https://github.com/Habibelmir/DataProcessingApi.git' // Git repository URL
            }
        }
        stage('SonarQube Analysis - Begin') {
            steps {
                withSonarQubeEnv('sonarqube') {
                    sh "dotnet ${SONAR_SCANNER_HOME}/SonarScanner.MSBuild.dll begin /k:\"DataProcessApi\""
                }
            }
        }
        stage('Build project') {
            steps {
                echo 'Compilation du projet...'
                sh 'dotnet build ProcessServices.csproj' // Command to build the .NET project
            }
        }
        stage('SonarQube Analysis - End') {
            steps {
                withSonarQubeEnv('sonarqube') {
                    sh "dotnet ${SONAR_SCANNER_HOME}/SonarScanner.MSBuild.dll end"
                }
            }
        }
        stage('Build Docker Image') {
            steps {
                sh 'docker build -t ${DOCKER_IMAGE} .'
            }
        }
        stage('Login to docker hub') {
            steps {
                sh 'echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin'
            }
        }
        stage('Push image to docker hub') {
            steps {
                sh 'docker push $DOCKER_IMAGE'
            }
        }
        stage('Deploy project') {
            steps {
                sh 'docker run -d -p 7074:80 -e ASPNETCORE_URLS="http://+" -v /var/jenkins_home/Uploads:/app/Uploads $DOCKER_IMAGE'
            }
        }
    }
    post {
        always {
            sh 'docker logout'
        }
    }
}