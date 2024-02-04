import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { MessageService } from 'src/app/services/message.service';

const passwordMatchingValidatior: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('passwordConfirm');

  return password?.value === confirmPassword?.value ? null : { notmatched: true };
};

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {
  signUpForm: FormGroup = new FormGroup({});

  constructor(private authService: AuthService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.signUpForm = new FormGroup({
      'userEmail': new FormControl('', [Validators.required, Validators.email]),
      'userPhone': new FormControl('', Validators.pattern('[0-9]{11}')),
      'password': new FormControl('', [Validators.required]),
      'passwordConfirm': new FormControl('', [Validators.required])
    }, {validators: passwordMatchingValidatior});
  }

  signUp() {
    this.authService.signUp(
      this.signUpForm.value.userEmail,
      this.signUpForm.value.password,
      this.signUpForm.value.userPhone)
      .then(() => this.messageService.sendMessage('Пользователь успешно зарегистрирован', 3000))
      .catch(err => {
        if (err.status === 400) {
          this.messageService.sendErrorMessage(err.error)
        }
        else {
          this.messageService.sendErrorMessage('Что-то пошло не так')
        }
      });
  }
}