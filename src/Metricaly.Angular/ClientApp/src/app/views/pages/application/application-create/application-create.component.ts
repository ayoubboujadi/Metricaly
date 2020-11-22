import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { NbDialogRef } from '@nebular/theme';

@Component({
  selector: 'app-application-create',
  templateUrl: './application-create.component.html',
  styleUrls: ['./application-create.component.css']
})
export class ApplicationCreateComponent implements OnInit {

  createApplicationForm = this.fb.group({
    applicationName: new FormControl('', [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(30),
    ])
  }, { updateOn: 'blur' });

  get applicationName() { return this.createApplicationForm.get('applicationName'); }

  constructor(private fb: FormBuilder, private dialogRef: NbDialogRef<ApplicationCreateComponent>) { }

  ngOnInit(): void {
  }

  onSubmit() {
    if (this.createApplicationForm.valid) {
      const appName = this.createApplicationForm.get('applicationName').value;
      this.dialogRef.close(appName);
    } else {
      this.validateAllFormFields(this.createApplicationForm);
    }
  }

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

}
