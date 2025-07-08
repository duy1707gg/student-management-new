pipeline {
    agent any

    environment {
        SOLUTION = 'student-management-new.sln'
        PROJECT = 'student-management-new.csproj'
        ARTIFACT_DIR = "${WORKSPACE}\\artifacts"
        DEPLOY_DIR = "C:\\inetpub\\wwwroot\\student-management-new"
        APP_POOL_NAME = "DefaultAppPool"
    }

    stages {
        stage('Checkout') {
            steps {
                echo '📥 Cloning source code from GitHub...'
                git url: 'https://github.com/duy1707gg/student-management-new.git'
            }
        }

        stage('Clean') {
            steps {
                echo '🧹 Cleaning artifacts folder...'
                bat "IF EXIST \"${ARTIFACT_DIR}\" rmdir /S /Q \"${ARTIFACT_DIR}\""
                bat "mkdir \"${ARTIFACT_DIR}\""
            }
        }

        stage('Restore') {
            steps {
                echo '🔧 Restoring packages...'
                bat "dotnet restore \"${SOLUTION}\""
            }
        }

        stage('Build') {
            steps {
                echo '⚙️ Building project...'
                bat "dotnet build \"${SOLUTION}\" --configuration Release --no-restore"
            }
        }

        stage('Publish') {
            steps {
                echo '📦 Publishing project...'
                bat "dotnet publish \"${PROJECT}\" -c Release -o \"${ARTIFACT_DIR}\""
            }
        }

        stage('Stop IIS AppPool') {
            steps {
                echo '⛔ Stopping AppPool and killing lock processes...'
                powershell(script: '''
                    try {
                        Import-Module WebAdministration -ErrorAction Stop
                        Stop-WebAppPool -Name "$env:APP_POOL_NAME" -ErrorAction SilentlyContinue
                        Start-Sleep -Seconds 3

                        $dllPath = "$env:DEPLOY_DIR\\student-management-new.dll"
                        $procs = Get-Process -ErrorAction SilentlyContinue | Where-Object {
                            $_.Modules | Where-Object { $_.FileName -eq $dllPath }
                        }

                        if ($procs) {
                            $procs | ForEach-Object {
                                Write-Output "⚠ Killing process $($_.ProcessName) that locked the DLL"
                                $_.Kill()
                            }
                        } else {
                            Write-Output "✔ No process locking DLL."
                        }
                    } catch {
                        Write-Warning "⚠ Error during AppPool stop: $_"
                    }
                    exit 0
                ''', returnStatus: true)
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo '🚀 Deploying to IIS...'
                powershell "New-Item -ItemType Directory -Force -Path \"${DEPLOY_DIR}\" | Out-Null"
                bat "robocopy \"${ARTIFACT_DIR}\" \"${DEPLOY_DIR}\" /E /Z /NP /NFL /NDL /R:3 /W:5"
            }
        }

        stage('Start IIS AppPool') {
            steps {
                echo '▶️ Starting IIS AppPool...'
                powershell(script: '''
                    try {
                        Import-Module WebAdministration -ErrorAction Stop
                        Start-WebAppPool -Name "$env:APP_POOL_NAME"
                    } catch {
                        Write-Warning "⚠ Failed to start AppPool: $_"
                        exit 0
                    }
                ''', returnStatus: true)
            }
        }

        stage('List deployed files') {
            steps {
                echo '📂 Listing deployed files...'
                bat "dir \"${DEPLOY_DIR}\""
            }
        }
    }

    post {
        success {
            echo '✅ CI/CD completed successfully.'
        }
        failure {
            echo '❌ CI/CD failed. Check the logs for issues.'
        }
    }
}
